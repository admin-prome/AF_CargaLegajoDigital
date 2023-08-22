using LegajoDigitalApp.Model;
using LegajoDigitalDemoApp.DAL;
//using LegajoDigitalDemoApp.Log;
using LegajoDigitalDemoApp.Model;
using LegajoDigitalDemoApp.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace LegajoDigitalApp.Business
{
    internal class BusinessLD
    {
        internal async static void ExecuteProccess(ServiceLD davoService, LDRecordForInsert record)
        {
            DataTable nifs = DataBaseManager.GetNIFSFromSourceTable();

            for (int i = 0; i < nifs.Rows.Count; i++)
            {
                
                string nif = nifs.Rows[i].Field<string>(0);

                ServiceResponse result = await davoService.GetResponseFromService(nif);
                if (result.IsSuccess)
                {
                    record = record.AdjuntarDatosDeRegistro(result.Result, nif);
                    DataBaseManager.InsertRecords(record);
                }
                else
                {
                    throw new Exception("Falla en servicio de Legajo Digital del Banco");    
                }

            }
        }
    }
}
