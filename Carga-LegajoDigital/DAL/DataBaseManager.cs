using LegajoDigitalApp.Model;
//using LegajoDigitalDemoApp.Log;
using LegajoDigitalDemoApp.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using System.Text;

namespace LegajoDigitalDemoApp.DAL
{
    internal class DataBaseManager
    {
        public DataBaseManager() 
        {
        }


        internal static DataTable GetNIFSFromInsertSourceTable()
        {
            
            DataTable records = ExecuteSourceSP();
            return records;
        }

        private static DataTable ExecuteSourceSP()
        {
            
            try 
            {
                var records = new DataTable();
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("216")))
                {
                    connection.Open();
                    SqlCommand Cmd = new SqlCommand("GetNewNifs", connection);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.CommandTimeout = 240;
                    DbDataReader rdr = Cmd.ExecuteReader();
                    records.Load(rdr);
                }
                return records;

            }
            catch(Exception e) 
            {
                throw new Exception("Hubo un error en el StoredProcedure de GetNif "+e.Message);
            }
        }

        
        internal static void InsertRecords(LDRecordForInsert recordForInsert)
        {
           try
            {
                Console.WriteLine("Insertando "+ recordForInsert.NIF);
                var records = new DataTable();
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("216")))
                {
                    connection.Open();
                    SqlCommand Cmd = new SqlCommand("Insert_in_LD_Tabla_Destino", connection);
               
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.Add("@NIF", SqlDbType.VarChar,25).Value = recordForInsert.NIF;
                    Cmd.Parameters.Add("@bp730", SqlDbType.VarChar,10).Value = recordForInsert.apiDavoServiceResponse.bp730;
                    Cmd.Parameters.Add("@bp733", SqlDbType.VarChar, 10).Value = recordForInsert.apiDavoServiceResponse.bp733;
                    Cmd.Parameters.Add("@bp510", SqlDbType.VarChar, 10).Value = recordForInsert.apiDavoServiceResponse.bp510;
                    Cmd.Parameters.Add("@dni_servicios", SqlDbType.VarChar, 10).Value = recordForInsert.apiDavoServiceResponse.dni_servicios;
                    Cmd.Parameters.Add("@bp935", SqlDbType.VarChar, 10).Value = recordForInsert.apiDavoServiceResponse.bp935;
                    Cmd.Parameters.Add("@fecha_consulta", SqlDbType.VarChar, 10).Value = recordForInsert.fechaConsulta;
                    Cmd.Parameters.Add("@cuil", SqlDbType.VarChar, 10).Value = recordForInsert.apiDavoServiceResponse.cuil;
                    Cmd.ExecuteNonQuery();

                }
                

            }
            catch (Exception e)
            {
                throw new Exception("Hubo un error en el StoredProcedure de insert" + e.Message);
            }
        }

        internal static DataTable GetNIFSFromUpdateSourceTable()
        {
            try
            {
                var records = new DataTable();
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("216")))
                {
                    connection.Open();
                    SqlCommand Cmd = new SqlCommand("GetUpdateNifs", connection);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    DbDataReader rdr = Cmd.ExecuteReader();
                    records.Load(rdr);
                }
                return records;

            }
            catch (Exception e)
            {
                throw new Exception("Hubo un error en el StoredProcedure de GetNif " + e.Message);
            }
        }

        internal static void UpdateRecord(LDRecordForInsert recordForInsert)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("216")))
                {
                    connection.Open();
                    SqlCommand Cmd = new SqlCommand("UpdateNif", connection);
                    Cmd.CommandType = CommandType.StoredProcedure;

                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.Add("@NIF", SqlDbType.VarChar, 25).Value = recordForInsert.NIF;
                    Cmd.Parameters.Add("@bp730", SqlDbType.VarChar, 10).Value = recordForInsert.apiDavoServiceResponse.bp730;
                    Cmd.Parameters.Add("@bp733", SqlDbType.VarChar, 10).Value = recordForInsert.apiDavoServiceResponse.bp733;
                    Cmd.Parameters.Add("@bp510", SqlDbType.VarChar, 10).Value = recordForInsert.apiDavoServiceResponse.bp510;
                    Cmd.Parameters.Add("@dni_servicios", SqlDbType.VarChar, 10).Value = recordForInsert.apiDavoServiceResponse.dni_servicios;
                    Cmd.Parameters.Add("@bp935", SqlDbType.VarChar, 10).Value = recordForInsert.apiDavoServiceResponse.bp935;
                    Cmd.Parameters.Add("@fecha_consulta", SqlDbType.VarChar, 10).Value = recordForInsert.fechaConsulta;
                    Cmd.Parameters.Add("@cuil", SqlDbType.VarChar, 10).Value = recordForInsert.apiDavoServiceResponse.cuil;
                    Cmd.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while calling the UpdateNif stored procedure: " + e.Message);
            }

        }
    }
}
