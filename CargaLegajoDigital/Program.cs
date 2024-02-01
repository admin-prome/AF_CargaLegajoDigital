using Azure.Identity;
using LegajoDigitalApp.Business;
using LegajoDigitalDemoApp.Service;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

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
                IConfiguration keyVaultConfiguration = GetKeyVaultConfiguration(configuration);
                string logFilePath = GetLogFilePath();
                RedirectConsoleOutputToFile(logFilePath);
                Task backgroundTask = ExecuteAsync(keyVaultConfiguration);
                backgroundTask.Wait();
                Console.WriteLine("Legajo Digital Console Application completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                SendFailureNotification(ex);
            }
        }

        private static void SendFailureNotification(Exception ex)
        {
            try
            {
                string apiUrl = configuration["FailureNotification:ApiEndpoint"];
                string idApp = configuration["FailureNotification:idApp"];

                var payload = new
                {
                    idApp = idApp,
                    errorMessage = ex.Message
                };

                string jsonPayload = JsonConvert.SerializeObject(payload);
                StringContent httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    // Send POST request to Azure Functions API
                    var response = httpClient.PostAsync(apiUrl, httpContent).Result;

                    // Check response status, log if needed
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Error sending failure notification: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                    else
                    {
                        Console.WriteLine("Failure notification sent successfully");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error sending failure notification: {e.Message}");
            }
        }
        static string GetLogFilePath()
        {
            string exeDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string logFolder = Path.Combine(exeDirectory, "Log");

            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }
            string logFileName = $"console_output_{DateTime.Now:yyyyMMdd_HHmmss}.log";
            return Path.Combine(logFolder, logFileName);
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
                throw;
            }
        }
    }
}
