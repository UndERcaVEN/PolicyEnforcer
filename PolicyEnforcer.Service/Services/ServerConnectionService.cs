using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PolicyEnforcer.Service.Configuration;
using PolicyEnforcer.Service.Models;
using PolicyEnforcer.Service.Services.Interfaces;
using System.Configuration;
using System.Net.Http.Json;
using System.Text;

namespace PolicyEnforcer.Service.Services
{
    public class ServerConnectionService : IServerConnectionService, IHostedService
    {
        private readonly IHardwareMonitoringService _monitoringService;
        private readonly IHistoryCollectionService _historyCollectionService;
        private readonly ILogger<ServerConnectionService> _logger;
        private readonly HttpClient _httpClient;
        private Guid _userID;
        private readonly POCO.LoginInfo _loginInfo;

        HubConnection _connection { get; set; }

        public ServerConnectionService(IHistoryCollectionService historyCollectionsvc, IHardwareMonitoringService monitoringsvc, ILogger<ServerConnectionService> logger, IOptions<POCO.LoginInfo> loginConfig) 
        {
            _monitoringService = monitoringsvc;
            _historyCollectionService = historyCollectionsvc;
            _logger = logger;
            _loginInfo = loginConfig.Value;

            var clientHandler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            };

            _httpClient = new HttpClient(clientHandler);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await ConfigureConnection();

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        /// <summary>
        /// Конфигурация подключения к серверу
        /// </summary>
        /// <returns></returns>
        private async Task ConfigureConnection()
        {
            var configFile = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;

            var loginInfo = await Login(settings);

            var url = settings["WorkingURL"]; // Адрес сервера хранится в конфиге

            _connection = new HubConnectionBuilder()
                .WithUrl($"https://26.85.180.83:6969/data",
                options =>
                {
                    options.UseDefaultCredentials = true;
                    options.HttpMessageHandlerFactory = (msg) =>
                    {
                        if (msg is HttpClientHandler clientHandler)
                        {
                            // bypass SSL certificate
                            clientHandler.ServerCertificateCustomValidationCallback +=
                                (sender, certificate, chain, sslPolicyErrors) => { return true; };
                        }

                        return msg;
                    };
                    options.AccessTokenProvider = () => Task.FromResult("Bearer " + loginInfo.Token);
                })
                .WithAutomaticReconnect()
                .AddNewtonsoftJsonProtocol(opts =>
                    opts.PayloadSerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto)
                .Build();

            await _connection.StartAsync();
            _logger.LogInformation($"Connection established at: {DateTimeOffset.Now}");

            _connection.On("GetHardwareInfo", GetTemps);
            _connection.On<int>("GetBrowserHistory", GetBrowserHistory);

            configFile.Save(ConfigurationSaveMode.Modified);
            System.Configuration.ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);

        }

        /// <summary>
        /// Авторизация в системе
        /// </summary>
        /// <param name="settings">конфиг</param>
        /// <returns></returns>
        private async Task<LoginInfo> Login(KeyValueConfigurationCollection? settings)
        {
            var filepath = Path.Combine(Environment.CurrentDirectory, "config.xml");
            if (!File.Exists(filepath))
            {
                var loginInfo = await Register(settings);
                return loginInfo;
            }

            var config = XMLHelper.FromXmlFile<LoginConfig>(filepath);

            var login = config.Login;
            var password = config.Password;

            var token = await RenewToken(new UserDTO(){ login = login, password = password });

            return new LoginInfo { Username = login, Password = password, Token = token };
        }

        /// <summary>
        /// Обновление токена
        /// </summary>
        /// <param name="userinfo">учетные данные</param>
        /// <returns></returns>
        private async Task<string> RenewToken(UserDTO userinfo)
        {
            var message = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = JsonContent.Create(userinfo),
                RequestUri = new UriBuilder("https://26.85.180.83:6969/api/Users/login").Uri
            };

            var response = _httpClient.Send(message);
            response.EnsureSuccessStatusCode();

            var token = await response.Content.ReadFromJsonAsync<TokenDTO>();
            this._userID = token.UserID;

            return token.Token;
        }
        

        /// <summary>
        /// Регистрация в системе
        /// </summary>
        /// <param name="settings">конфиг</param>
        /// <returns></returns>
        private async Task<LoginInfo> Register(KeyValueConfigurationCollection? settings)
        {
            var loginInfo = LoginInfo.Generate();

            var content = new UserDTO() { login = loginInfo.Username, password = loginInfo.Password };

            string json = JsonConvert.SerializeObject(content);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var message = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = httpContent,
                RequestUri = new UriBuilder("https://26.85.180.83:6969/api/Users/register").Uri
            };

            var response = _httpClient.Send(message);

            response.EnsureSuccessStatusCode();

            var token = await response.Content.ReadFromJsonAsync<TokenDTO>();

            this._userID = token.UserID;
            loginInfo.Token = token.Token;

            var loginConfig = new LoginConfig
            {
                Login = loginInfo.Username,
                Password = loginInfo.Password,
                Token = token.Token,
            };

            var filepath = Path.Combine(Environment.CurrentDirectory, "config.xml");
            XMLHelper.ToXmlFile(loginConfig, filepath);

            return loginInfo;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Service stopped at {DateTime.Now}");
            await Task.CompletedTask;
        }

        /// <summary>
        /// Опрос АО
        /// </summary>
        public async void GetTemps()
        {
            _logger.LogInformation($"Received hardware poll request at {DateTimeOffset.Now}");
            var readings = _monitoringService.PollHardware(_userID);

            await _connection.InvokeAsync("ReturnHardwareReadings", readings);
        }

        /// <summary>
        /// Сбор истории браузера
        /// </summary>
        /// <param name="batchSize">количество записей в пакете данных</param>
        public async void GetBrowserHistory(int batchSize = 10)
        {
            _logger.LogInformation($"Received history collection request at {DateTimeOffset.Now}");
            var readings = _historyCollectionService.GetBrowsersHistory(DateTime.Now.AddDays(-2), _userID);

            while (readings.Count > 0)
            {
                var ceiling = readings.Count > batchSize ? batchSize : readings.Count;
                _connection.InvokeAsync("ReturnBrowserHistory", readings.GetRange(0, ceiling));
                readings.RemoveRange(0, ceiling);
            }
        }
    }
}
