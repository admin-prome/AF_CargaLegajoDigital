using LegajoDigitalApp.Model;
using LegajoDigitalDemoApp.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace LegajoDigitalDemoApp.DAL
{
    internal class DataBaseManager
    {
        private readonly IConfiguration configuration;

        internal DataBaseManager(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        internal DataTable GetNIFSFromInsertSourceTable()
        {
            Console.WriteLine("Entrando al GetNifsFrom...");
            DataTable records = ExecuteSourceSP();
            return records;
        }

        private DataTable ExecuteSourceSP()
        {
            try
            {
                var records = new DataTable();
                Console.WriteLine("SP");
                using (SqlConnection connection = new SqlConnection(configuration["DataWarehouse-1"]))
                {
                    Console.WriteLine("Connection Open?");
                    int timeout = connection.ConnectionTimeout;
                    Console.WriteLine("Connection Timeout " + timeout);
                    connection.Open();
                    Console.WriteLine("Connection Open");
                    SqlCommand Cmd = new SqlCommand("GetNewNifs", connection);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.CommandTimeout = 240;
                    DbDataReader rdr = Cmd.ExecuteReader();
                    Console.WriteLine("Execute Reader");
                    records.Load(rdr);
                }
                Console.WriteLine("Saliendo");
                return records;
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
                throw new Exception("Hubo un error en el StoredProcedure de GetNif " + e.Message);
            }
        }

        internal void InsertRecords(LDRecordForInsert recordForInsert)
        {
            try
            {
                Console.WriteLine("Insertando " + recordForInsert.NIF);
                var records = new DataTable();
                using (SqlConnection connection = new SqlConnection(configuration["DataWarehouse-1"]))
                {
                    connection.Open();
                    SqlCommand Cmd = new SqlCommand("Insert_in_LD_Tabla_Destino", connection);

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
                throw new Exception("Hubo un error en el StoredProcedure de insert" + e.Message);
            }
        }

        internal DataTable GetNIFSFromUpdateSourceTable()
        {
            try
            {
                var records = new DataTable();
                using (SqlConnection connection = new SqlConnection(configuration["DataWarehouse-1"]))
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

        internal void UpdateRecord(LDRecordForInsert recordForInsert)
        {
            Console.WriteLine("Actualizando " + recordForInsert.NIF);
            try
            {
                using (SqlConnection connection = new SqlConnection(configuration["DataWarehouse-1"]))
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

        internal void UpdateRecordsState()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(configuration["DataWarehouse-1"]))
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

        internal DataTable GetTableData()
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(configuration["DataWarehouse-1"]))
                {
                    connection.Open();
                    string selectQuery = "SELECT * FROM LD_TABLA_DESTINO"; 
                    SqlCommand selectCmd = new SqlCommand(selectQuery, connection);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(selectCmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error retrieving table data: " + e.Message);
            }

            return dataTable;
        }


    }
}
