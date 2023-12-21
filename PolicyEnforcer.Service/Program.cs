using PolicyEnforcer.Service.Services;
using PolicyEnforcer.Service.Services.Interfaces;
using System.Runtime.InteropServices;

internal class Program
{
    public static IConfigurationRoot Configuration { get; private set; }

    private static void Main(string[] args)
    {
        var configBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json");

        Configuration = configBuilder.Build();

        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddHostedService<ServerConnectionService>();
                services.AddTransient<IHistoryCollectionService, HistoryCollectionService>();
                services.AddTransient<IHardwareMonitoringService, HardwareMonitoringService>();
                services.Configure<PolicyEnforcer.Service.POCO.LoginInfo>(Configuration.GetSection("LoginInfo"));
            })
            .ConfigureLogging((context, logging) =>
            {
                logging.ClearProviders();
                logging.AddConfiguration(context.Configuration.GetSection("Logging"));

                if (context.HostingEnvironment.IsDevelopment())
                {
                    AllocConsole();
                    logging.AddConsole();
                    logging.AddDebug();
                }
            })
            .Build();

        host.Run();
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool AllocConsole();
}