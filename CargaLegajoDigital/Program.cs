﻿using Azure.Identity;
using LegajoDigitalApp.Business;
using LegajoDigitalDemoApp.Service;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CargaLegajoDigital
{
    class Program
    {
        private static IConfiguration configuration;

        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting Legajo Digital Console Application");
                configuration = GetConfiguration();
                IConfiguration keyVaultConfiguration = GetKeyVaultConfiguration(configuration);
                Task backgroundTask = ExecuteAsync(keyVaultConfiguration);
                await backgroundTask;

                Console.WriteLine("Legajo Digital Console Application completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static IConfiguration GetConfiguration()
        {
            try
            {
                Console.WriteLine("Loading configuration...");

                IConfiguration localConfiguration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
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