using LegajoDigitalDemoApp.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LegajoDigitalDemoApp.Service
{
    internal class ServiceLD
    {
        private readonly HttpClient httpClient;
        private readonly string baseUrl;
        private readonly IConfiguration configuration;
        private readonly int maxRetryAttempts = 3;
        private readonly int delayMs = 1000;

        public ServiceLD(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
            this.baseUrl = configuration["urlServicioBancoLegajoDigital"];
        }

        public async Task<ServiceResponse> GetResponseFromService(Int64 nif)
        {
            for (int attempt = 0; attempt < maxRetryAttempts; attempt++)
            {
                try
                {
                    string nifString = nif.ToString();
                    HttpResponseMessage ApiResponse = await httpClient.GetAsync(this.baseUrl + nifString);
                    ApiResponse.EnsureSuccessStatusCode();
                    string responseBody = await ApiResponse.Content.ReadAsStringAsync();
                    LDServiceResponse ApiPokemonResponse = JsonConvert.DeserializeObject<LDServiceResponse>(responseBody);
                    ServiceResponse serviceResponse = GenerateResponse(ApiPokemonResponse);
                    return serviceResponse;
                }
                catch (HttpRequestException ex) when (attempt < maxRetryAttempts - 1)
                {
                    Console.WriteLine($"Error occurred: {ex.Message}. Retrying...");
                    await Task.Delay(delayMs);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Max retry attempts reached: {ex.Message}");
                }
            }

            throw new Exception("Max retry attempts reached without success.");
        }

        private ServiceResponse GenerateBadResponse(string message)
        {

            ServiceResponse res = new ServiceResponse();
            res.Result = null;
            res.ErrorMessages.Add(message);
            res.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            res.IsSuccess = false;
            return res;
        }

        private ServiceResponse GenerateResponse(LDServiceResponse apiPokemonResponse)
        {
            ServiceResponse res = new ServiceResponse();
            res.Result = apiPokemonResponse;
            res.StatusCode = System.Net.HttpStatusCode.OK;
            res.IsSuccess = true;
            return res;
        }
    }
}
