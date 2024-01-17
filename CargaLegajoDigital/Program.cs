using Azure.Identity;
using LegajoDigitalApp.Business;
using LegajoDigitalDemoApp.Service;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace CargaLegajoDigital
{
    class Program
    {
        private static IConfiguration configuration;

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting Legajo Digital Console Application");
                configuration = GetConfiguration();
                string logFilePath = GetLogFilePath();
                RedirectConsoleOutputToFile(logFilePath);
                IConfiguration keyVaultConfiguration = GetKeyVaultConfiguration(configuration);
                Task backgroundTask = ExecuteAsync(keyVaultConfiguration);
                backgroundTask.Wait();
                Console.WriteLine("Legajo Digital Console Application completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        static string GetLogFilePath()
        {
            string exeDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string logFileName = $"console_output_{DateTime.Now:yyyyMMdd_HHmmss}.log";
            return Path.Combine(exeDirectory, logFileName);
        }

        static void RedirectConsoleOutputToFile(string logFilePath)
        {
            try
            {
                var fileStream = new FileStream(logFilePath, FileMode.Append, FileAccess.Write);
                var streamWriter = new StreamWriter(fileStream);
                streamWriter.AutoFlush = true;
                Console.SetOut(streamWriter);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error redirecting console output to file: {ex.Message}");
            }
        }

        static IConfiguration GetConfiguration()
        {
            try
            {
                Console.WriteLine("Loading configuration...");

                string exeDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

                IConfiguration localConfiguration = new ConfigurationBuilder()
                    .SetBasePath(exeDirectory)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();


                Console.WriteLine("Configuration loaded.");

                return localConfiguration;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading configuration: {ex.Message}");
                throw;
            }
        }

        static IConfiguration GetKeyVaultConfiguration(IConfiguration localConfiguration)
        {
            try
            {
                Console.WriteLine("Creating Azure Key Vault configuration...");

                var keyVaultEndpoint = new Uri("https://tecnokeys.vault.azure.net/");
                string tenantId = localConfiguration["ServicePrincipal:tenantId"];
                string clientId = localConfiguration["ServicePrincipal:clientId"];
                string clientSecret = localConfiguration["ServicePrincipal:clientSecret"];
                var client = new ClientSecretCredential(tenantId, clientId, clientSecret);
                var configurationBuilder = new ConfigurationBuilder()
                    .AddConfiguration(localConfiguration) 
                    .AddAzureKeyVault(keyVaultEndpoint, client);
                var configurationKeyVault = configurationBuilder.Build();

                Console.WriteLine("Azure Key Vault configuration created.");

                return configurationKeyVault;
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
                ServiceLD LDService = new ServiceLD(configuration);
                await business.ExecuteProccess(LDService);
                Console.WriteLine("Execution completed");

            }
            catch (Exception e)
            {
                Console.WriteLine($"Legajo Digital process failed: {DateTime.Now} {e.Message}");
            }
        }
    }
}
