using System;
using LegajoDigitalApp.Business;
using LegajoDigitalApp.Model;
using LegajoDigitalDemoApp.Service;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Carga_LegajoDigital
{
    public class LoadFunction
    {
        [FunctionName("LoadFunction")]
        public void Run([TimerTrigger("0 00 13 * * *")]TimerInfo myTimer, ILogger log)
        {
            try
            {
                /**/
                log.LogInformation("Entrando a Azure Function");
                
                ServiceLD LDService = new ServiceLD();
                BusinessLD.ExecuteProccess(LDService,log);
                
                log.LogInformation("Se ejecut� correctamente");

            }
            catch (Exception e)
            {
                log.LogInformation($"Fall� proceso de Legajo Digital: {DateTime.Now}" + " " + e.Message);
            }
           
        }
    }
}
