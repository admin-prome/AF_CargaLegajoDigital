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
        public void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            try
            {
                /*
                ServiceLD LDService = new ServiceLD();
                LDRecordForInsert record = new LDRecordForInsert();
                BusinessLD.ExecuteProccess(LDService, record);
                */
                Console.WriteLine("Hola");
                log.LogInformation("Se ejecutó correctamente");

            }
            catch (Exception e)
            {
                log.LogInformation($"Falló proceso de Legajo Digital: {DateTime.Now}" + " " + e.Message);
             
            }
           
        }
    }
}
