using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

public class BlobStorageHelper
{
    private static CloudBlobContainer GetContainer()
    {
        string connectionString = ConfigurationManager.AppSettings["AzureStorageConnectionString"];
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
        return blobClient.GetContainerReference("uploads");
    }

    public static string UploadFile(Stream fileStream, string fileName, string containerName)
    {
        string connectionString = ConfigurationManager.AppSettings["AzureStorageConnectionString"];
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
        CloudBlobContainer container = blobClient.GetContainerReference(containerName);
        CloudBlockBlob blob = container.GetBlockBlobReference(fileName);
        blob.UploadFromStream(fileStream);
        return blob.Uri.ToString();
    }

    public static void DeleteFile(string fileName, string containerName)
    {
        string connectionString = ConfigurationManager.AppSettings["AzureStorageConnectionString"];
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
        CloudBlobContainer container = blobClient.GetContainerReference(containerName);
        CloudBlockBlob blob = container.GetBlockBlobReference(fileName);
        blob.DeleteIfExists();
    }
}