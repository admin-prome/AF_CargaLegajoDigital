using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargaLegajoDigital.Helper
{
    public class TableExportHelper
    {
        public void ExportTableToTxt(DataTable records, string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine(string.Join(";", records.Columns.Cast<DataColumn>().Select(col => col.ColumnName)));

                    foreach (DataRow row in records.Rows)
                    {
                        writer.WriteLine(string.Join(";", row.ItemArray));
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error exporting table data: " + e.Message);
            }
        }
    }
}
