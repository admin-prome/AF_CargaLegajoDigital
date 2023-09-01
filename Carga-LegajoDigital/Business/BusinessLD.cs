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
        internal async static void ExecuteProccess(ServiceLD ldService, Microsoft.Extensions.Logging.ILogger log)
        {

            InsertNewRecords(ldService, log);
            UpdateRecords(ldService,log);
            UpdateRecordsState();
       
          
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
            DataTable nifs = DataBaseManager.GetNIFSFromInsertSourceTable(log);
            log.LogInformation("Salió del SP al 216");
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
                    log.LogInformation("Error en servicio de banco para registros.");
                }


            }
            if (fallaServicioCounter > 0)
            {
                log.LogInformation("Error en servicio de banco para " + fallaServicioCounter + " registros.");
            }
        }

        internal static void ConnectToProvMicroSQL(Microsoft.Extensions.Logging.ILogger log)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("provmicrosql02")))
                {
                    connection.Open();

                    log.LogInformation("Conexión hecha");
                    log.LogInformation(connection.State.ToString());
                }
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while calling the UpdateLDRecordsState stored procedure: " + e.Message);
            }
        }
    }
}
