using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Data;
using System.IO;
using System.Linq;

namespace CargaLegajoDigital.Helper
{
    public class StorageHelper
    {
        private readonly string storageConnectionString;
        private readonly string containerName;

        public StorageHelper(string storageConnectionString, string containerName)
        {
            this.storageConnectionString = storageConnectionString;
            this.containerName = containerName;
        }

        public void UploadTableDataToBlob(DataTable records, string blobName)
        {
            int maxRetries = 3;
            int retryCount = 0;
            bool uploadedSuccessfully = false;

            while (!uploadedSuccessfully && retryCount < maxRetries)
            {
                try
                {
                    Uri connectionString = new Uri(storageConnectionString);
                    BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString, null);
                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                    string blobNameTxt = blobName + ".txt";

                    using (MemoryStream stream = new MemoryStream())
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.WriteLine(string.Join(";", records.Columns.Cast<DataColumn>().Select(col => col.ColumnName)));

                            foreach (DataRow row in records.Rows)
                            {
                                writer.WriteLine(string.Join(";", row.ItemArray));
                            }
                            writer.Flush();
                            stream.Position = 0;
                            containerClient.UploadBlob(blobNameTxt, stream);
                            BlobHttpHeaders headers = new BlobHttpHeaders { ContentType = "text/plain" };
                            containerClient.GetBlobClient(blobNameTxt).SetHttpHeaders(headers);
                        }
                    }

                    uploadedSuccessfully = true; 
                }
                catch (Exception e)
                {
                    retryCount++;
                    if (retryCount < maxRetries)
                    {
                        Console.WriteLine($"Error uploading table data to blob (Retry {retryCount}/{maxRetries}): {e.Message}");
                    }
                    else
                    {
                        throw new Exception($"Error uploading table data to blob after {maxRetries} attempts: {e.Message}");
                    }
                }
            }
        }
    }
}
