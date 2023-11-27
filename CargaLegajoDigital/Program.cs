using Azure.Identity;
using LegajoDigitalApp.Business;
using LegajoDigitalDemoApp.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CargaLegajoDigital
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting Legajo Digital Console Application");
                IConfiguration configuration = GetKeyVaultConfiguration();
                Task backgroundTask = ExecuteAsync(configuration);
                Console.WriteLine("Press any key to stop the application.");
                Console.ReadKey();
                await backgroundTask;

                Console.WriteLine("Legajo Digital Console Application completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static IConfiguration GetKeyVaultConfiguration()
        {
            try
            {
                Console.WriteLine("Creating Azure Key Vault configuration...");

                var keyVaultEndpoint = new Uri("https://tecnokeys.vault.azure.net/");
                var configurationBuilder = new ConfigurationBuilder()
                    .AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());

                var configuration = configurationBuilder.Build();

                Console.WriteLine("Azure Key Vault configuration created.");

                return configuration;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating Azure Key Vault configuration: {ex.Message}");
                throw;
            }
        }

        static async Task ExecuteAsync(IConfiguration configuration)
        {
            try
            {
                Console.WriteLine("Entering ExecuteAsync");
                BusinessLD business = new BusinessLD(configuration);
                business.ConnectToProvMicroSQL();
                ServiceLD LDService = new ServiceLD(configuration);
                business.ExecuteProccess(LDService);
                Console.WriteLine("Execution completed");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Legajo Digital process failed: {DateTime.Now} {e.Message}");
            }
        }
    }
}
