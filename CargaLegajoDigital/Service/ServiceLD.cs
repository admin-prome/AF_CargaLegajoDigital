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
        private HttpClient client;
        private string baseUrl;
        private readonly IConfiguration configuration;

        public ServiceLD(IConfiguration configuration)
        {
            this.client = new HttpClient();
            this.configuration = configuration;
            this.baseUrl = configuration["urlServicioBancoLegajoDigital"];
        }

        public async Task<ServiceResponse> GetResponseFromService(Int64 nif)
        {
            try
            {
                string nifString = nif.ToString();
                HttpResponseMessage ApiResponse = await client.GetAsync(this.baseUrl + nifString);
                ApiResponse.EnsureSuccessStatusCode();
                string responseBody = await ApiResponse.Content.ReadAsStringAsync();
                LDServiceResponse ApiPokemonResponse = JsonConvert.DeserializeObject<LDServiceResponse>(responseBody);
                ServiceResponse serviceResponse = GenerateResponse(ApiPokemonResponse);
                return serviceResponse;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
              
            }


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
