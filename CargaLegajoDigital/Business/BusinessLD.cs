using LegajoDigitalApp.Model;
using LegajoDigitalDemoApp.DAL;
using LegajoDigitalDemoApp.Model;
using LegajoDigitalDemoApp.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Data.SqlClient;

namespace LegajoDigitalApp.Business
{
    internal class BusinessLD
    {
        private readonly IConfiguration configuration;

        internal BusinessLD(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        internal async void ExecuteProccess(ServiceLD ldService, Microsoft.Extensions.Logging.ILogger log)
        {
            await InsertNewRecords(ldService, log);
            await UpdateRecords(ldService, log);
            UpdateRecordsState();
        }

        private void UpdateRecordsState()
        {
            DataBaseManager dataBaseManager = new DataBaseManager(configuration);
            dataBaseManager.UpdateRecordsState();
        }

        private async Task UpdateRecords(ServiceLD ldService, Microsoft.Extensions.Logging.ILogger log)
        {
            DataBaseManager dataBaseManager = new DataBaseManager(configuration);
            dataBaseManager.UpdateRecordsState();
            DataTable nifs = dataBaseManager.GetNIFSFromUpdateSourceTable();
            int fallaServicioCounter = 0;

            for (int i = 0; i < nifs.Rows.Count; i++)
            {
                Console.WriteLine("Actualizando " + i);
                Int64 nif = nifs.Rows[i].Field<Int64>(0);

                ServiceResponse result = await ldService.GetResponseFromService(nif);

                if (result.IsSuccess)
                {
                    LDRecordForInsert record = new LDRecordForInsert();
                    record = record.AdjuntarDatosDeRegistro(result.Result, nif);
                    dataBaseManager.UpdateRecord(record);
                }
                else
                {
                    fallaServicioCounter++;
                }
            }

            if (fallaServicioCounter > 0)
            {
                Console.WriteLine("Error en servicio de banco para " + fallaServicioCounter + " registros.");
            }
        }

        private async Task InsertNewRecords(ServiceLD ldService, Microsoft.Extensions.Logging.ILogger log)
        {
            Console.WriteLine("Entrando a Insert New Records");
            LDRecordForInsert record = new LDRecordForInsert();
            DataBaseManager dataBaseManager = new DataBaseManager(configuration);
            DataTable nifs = dataBaseManager.GetNIFSFromInsertSourceTable(log);
            Console.WriteLine("Salió del SP al 216");
            int fallaServicioCounter = 0;

            for (int i = 0; i < nifs.Rows.Count; i++)
            {
                Console.WriteLine("Procesando " + i);
                Int64 nif = nifs.Rows[i].Field<Int64>(0);

                ServiceResponse result = await ldService.GetResponseFromService(nif);

                if (result.IsSuccess)
                {
                    record = record.AdjuntarDatosDeRegistro(result.Result, nif);
                    dataBaseManager.InsertRecords(record);
                }
                else
                {
                    Console.WriteLine("Error en servicio de banco para registros.");
                }
            }

            if (fallaServicioCounter > 0)
            {
                Console.WriteLine("Error en servicio de banco para " + fallaServicioCounter + " registros.");
            }
        }

        internal void ConnectToProvMicroSQL(Microsoft.Extensions.Logging.ILogger log)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(configuration["DW2-ProvMicroDesa"]))
                {
                    connection.Open();
                    Console.WriteLine("Conexión hecha");
                    Console.WriteLine(connection.State.ToString());
                }
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while calling the UpdateLDRecordsState stored procedure: " + e.Message);
            }
        }
    }
}
