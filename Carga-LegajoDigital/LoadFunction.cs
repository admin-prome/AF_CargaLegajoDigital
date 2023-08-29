using System;
using System.Threading.Tasks;
using System.Web.Http;
using LegajoDigitalApp.Business;
using LegajoDigitalApp.Model;
using LegajoDigitalDemoApp.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Carga_LegajoDigital
{
    public class LoadFunction
    {
        [FunctionName("LoadFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ILogger log)
        {
            try
            {
                string message = String.Empty;
                /**/
                try 
                {
                    log.LogInformation("Entrando a Azure Function");

                    ServiceLD LDService = new ServiceLD();
                    BusinessLD.ExecuteProccess(LDService, log,message);

                    log.LogInformation("Se ejecutó correctamente");
                }
                catch (Exception ex) 
                {
                  message= ex.Message;
                }
           
                return new OkObjectResult("Se ejecutó correctamente: " + message);

            }
            catch (Exception e)
            {
                log.LogInformation($"Falló proceso de Legajo Digital: {DateTime.Now}" + " " + e.Message);
                return new BadRequestObjectResult(" " + e.Message);
            }
           
        }
    }
}
