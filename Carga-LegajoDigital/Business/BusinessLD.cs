﻿using LegajoDigitalApp.Model;
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
           
            InsertNewRecords(ldService,log);
            UpdateRecords(ldService,log);
            UpdateRecordsState();
        }

        private static void UpdateRecordsState()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("216")))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("UpdateLDRecordsState", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while calling the UpdateLDRecordsState stored procedure: " + e.Message);
            }

        }

        private static async void UpdateRecords(ServiceLD ldService, Microsoft.Extensions.Logging.ILogger log)
        {
            DataTable nifs = DataBaseManager.GetNIFSFromUpdateSourceTable();
            int fallaServicioCounter = 0;
            for (int i = 0; i < nifs.Rows.Count; i++)
            {
                
                string nif = nifs.Rows[i].Field<string>(0);

                ServiceResponse result = await ldService.GetResponseFromService(nif);
                 //Busco registro en Tabla Destino y actualizo
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
            LDRecordForInsert record = new LDRecordForInsert();
            DataTable nifs = DataBaseManager.GetNIFSFromInsertSourceTable();
            int fallaServicioCounter = 0;
            for (int i = 0; i < nifs.Rows.Count; i++)
            {

                string nif = nifs.Rows[i].Field<Int64>(0).ToString();

                ServiceResponse result = await ldService.GetResponseFromService(nif);
                if (result.IsSuccess)
                {
                    record = record.AdjuntarDatosDeRegistro(result.Result, nif);
                    DataBaseManager.InsertRecords(record);
                }
                else
                {
                    fallaServicioCounter++;
                }


            }
            if (fallaServicioCounter > 0)
            {
                log.LogInformation("Error en servicio de banco para " + fallaServicioCounter + " registros.");
            }
        }
    }
}
