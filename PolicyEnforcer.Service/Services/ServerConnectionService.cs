using Microsoft.AspNetCore.SignalR.Client;
using PolicyEnforcer.Service.Models;
using PolicyEnforcer.Service.Services.Interfaces;
using System.Configuration;
using System.Net.Http.Json;

namespace PolicyEnforcer.Service.Services
{
    public class ServerConnectionService : IServerConnectionService, IHostedService
    {
        private readonly IHardwareMonitoringService _monitoringService;
        private readonly IHistoryCollectionService _historyCollectionService;
        private readonly ILogger<ServerConnectionService> _logger;
        private readonly HttpClient _httpClient;

        HubConnection _connection { get; set; }

        public ServerConnectionService(IHistoryCollectionService historyCollectionsvc, IHardwareMonitoringService monitoringsvc, ILogger<ServerConnectionService> logger) 
        {
            _monitoringService = monitoringsvc;
            _historyCollectionService = historyCollectionsvc;
            _logger = logger;

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
                var configFile = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;

                var loginInfo = await Login(settings);

                var url = settings["WorkingURL"];

                _connection = new HubConnectionBuilder()
                    .WithUrl($"{url}/data",
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
                        options.AccessTokenProvider = () => Task.FromResult(loginInfo.Token);
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

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private async Task<LoginInfo> Login(KeyValueConfigurationCollection? settings)
        {
            var login = settings["LoginInfo:Login"].Value;
            if (login == "default")
            {
                var loginInfo = await Register(settings);
                return loginInfo;
            }

            var password = settings["LoginInfo:Password"].Value;

            var token = await RenewToken(new UserDTO(){ login = login, password = password });
            settings["LoginInfo:Token"].Value = token;

            

            return new LoginInfo { Username = login, Password = password, Token = token };
        }

        private async Task<string> RenewToken(UserDTO userinfo)
        {
            var message = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = JsonContent.Create(userinfo),
                RequestUri = new UriBuilder("https://192.168.0.102:6969/api/Admin/login").Uri
            };

            var response = _httpClient.Send(message);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<string>();
        }
        
        private async Task<LoginInfo> Register(KeyValueConfigurationCollection? settings)
        {
            var loginInfo = LoginInfo.Generate();

            var content = new UserDTO() { login = loginInfo.Username, password = loginInfo.Password };
            var message = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = JsonContent.Create(content),
                RequestUri = new UriBuilder("https://192.168.0.102:6969/api/Admin/register").Uri
            };

            var response = _httpClient.Send(message);

            response.EnsureSuccessStatusCode();

            var token = await response.Content.ReadFromJsonAsync<TokenDTO>();
            loginInfo.Token = token.Token;

            settings["Login"].Value = loginInfo.Username;
            settings["Password"].Value = loginInfo.Password;
            settings["Token"].Value = loginInfo.Token;

            return loginInfo;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Service stopped at {DateTime.Now}");
            await Task.CompletedTask;
        }


        public async void GetTemps()
        {
            _logger.LogInformation($"Received hardware poll request at {DateTimeOffset.Now}");
            var readings = _monitoringService.PollHardware();

            await _connection.InvokeAsync("ReturnHardwareReadings", readings);
        }

        public async void GetBrowserHistory(int batchSize = 10)
        {
            _logger.LogInformation($"Received history collection request at {DateTimeOffset.Now}");
            var readings = _historyCollectionService.GetBrowsersHistory(DateTime.Now.AddDays(-2));

            while (readings.Count > 0)
            {
                var ceiling = readings.Count > batchSize ? batchSize : readings.Count;
                _connection.InvokeAsync("ReturnBrowserHistory", readings.GetRange(0, ceiling));
                readings.RemoveRange(0, ceiling);
            }
        }
    }
}
