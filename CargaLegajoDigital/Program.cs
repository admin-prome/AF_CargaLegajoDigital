using Azure.Identity;
using LegajoDigitalApp.Business;
using LegajoDigitalDemoApp.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CargaLegajoDigital
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
     Host.CreateDefaultBuilder(args).UseWindowsService()
         .ConfigureServices((hostContext, services) =>
         {
             var keyVaultEndpoint = new Uri("https://tecnokeys.vault.azure.net/");
             services.AddAzureKeyVaultConfiguration(keyVaultEndpoint);
             services.AddHostedService<LoadService>();
         });

    }
    public static class ServiceCollectionExtensions
    {
        public static void AddAzureKeyVaultConfiguration(this IServiceCollection services, Uri keyVaultEndpoint)
        {
            try
            {
                Console.WriteLine("Creating Azure Key Vault configuration...");

                var configurationBuilder = new ConfigurationBuilder()
                    .AddAzureKeyVault(
                        keyVaultEndpoint,
                        new DefaultAzureCredential());

                var configuration = configurationBuilder.Build();

                Console.WriteLine("Azure Key Vault configuration created.");

                services.AddSingleton<IConfiguration>(configuration);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating Azure Key Vault configuration: {ex.Message}");
                throw; 
            }
        }
    }

    public class LoadService : BackgroundService
    {
        private readonly ILogger<LoadService> log;
        private readonly IConfiguration configuration;

        public LoadService(ILogger<LoadService> logger, IConfiguration configuration)
        {
            log = logger;
            this.configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                Console.WriteLine("Entering Windows Service");

                BusinessLD business = new BusinessLD(configuration);
                business.ConnectToProvMicroSQL(log);
                ServiceLD LDService = new ServiceLD(configuration);
                business.ExecuteProccess(LDService, log);

                Console.WriteLine("Execution succeeded");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Legajo Digital process failed: {DateTime.Now} {e.Message}");
            }
        }
    }
}

