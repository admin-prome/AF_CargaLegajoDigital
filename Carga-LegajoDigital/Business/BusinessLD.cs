using LegajoDigitalApp.Model;
using LegajoDigitalDemoApp.DAL;
//using LegajoDigitalDemoApp.Log;
using LegajoDigitalDemoApp.Model;
using LegajoDigitalDemoApp.Service;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace LegajoDigitalApp.Business
{
    internal class BusinessLD
    {
        internal async static void ExecuteProccess(ServiceLD ldService, Microsoft.Extensions.Logging.ILogger log, string message)
        {

            InsertNewRecords(ldService, log);
            message += "Pasó el InsertNewRecords" + "\n";
            UpdateRecords(ldService,log);
            message += "Pasó el UpdateRecords" + "\n";
            UpdateRecordsState();
            message += "Pasó el UpdateStateRecords" + "\n";
            throw new Exception(message);
        }

        private static void UpdateRecordsState()
        {
            DataBaseManager.UpdateRecordsState();

        }

        private static async void UpdateRecords(ServiceLD ldService, Microsoft.Extensions.Logging.ILogger log)
        {
            DataBaseManager.UpdateRecordsState();
            DataTable nifs = DataBaseManager.GetNIFSFromUpdateSourceTable();
            int fallaServicioCounter = 0;
            for (int i = 0; i < nifs.Rows.Count; i++)
            {
                log.LogInformation("Actualizando " + i);   
                Int64 nif = nifs.Rows[i].Field<Int64>(0);

                ServiceResponse result = await ldService.GetResponseFromService(nif);
                 
                if (result.IsSuccess)
                {
                    LDRecordForInsert record= new LDRecordForInsert();
                    record = record.AdjuntarDatosDeRegistro(result.Result, nif);
                    DataBaseManager.UpdateRecord(record);
                }
                else
                {
                    fallaServicioCounter++;
                }

            }
            if (fallaServicioCounter > 0) 
            {
                log.LogInformation("Error en servicio de banco para " + fallaServicioCounter +  " registros.");
            }
            
        }

        private static async void InsertNewRecords(ServiceLD ldService, Microsoft.Extensions.Logging.ILogger log)
        {
            log.LogInformation("Entrando a Insert New Records");
            LDRecordForInsert record = new LDRecordForInsert();
            DataTable nifs = DataBaseManager.GetNIFSFromInsertSourceTable();
            int fallaServicioCounter = 0;
            for (int i = 0; i < nifs.Rows.Count; i++)
            {
                log.LogInformation("Procesando " + i);
                Int64 nif = nifs.Rows[i].Field<Int64>(0);

                ServiceResponse result = await ldService.GetResponseFromService(nif);
                if (result.IsSuccess)
                {
                    record = record.AdjuntarDatosDeRegistro(result.Result, nif);
                    DataBaseManager.InsertRecords(record);
                }
                else
                {
                    throw new Exception("Falla en respuesta de servicio de banco");
                }


            }
            if (fallaServicioCounter > 0)
            {
                log.LogInformation("Error en servicio de banco para " + fallaServicioCounter + " registros.");
            }
        }
    }
}
