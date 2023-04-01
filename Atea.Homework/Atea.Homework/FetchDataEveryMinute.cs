using System;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Atea.Homework;

public static class FetchDataEveryMinute
{
    [FunctionName("FetchDataEveryMinute")]
    public static async Task RunAsync([TimerTrigger("0 * * * * *")] TimerInfo myTimer, ILogger log)
    {
        log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");

        // Handle blob stuff
        var containerName = "my-container";
        var endpoint = "https://api.publicapis.org/random?auth=null";
        
        var blobConnectionString = "UseDevelopmentStorage=true;DevelopmentStorageProxyUri=http://localhost:10000";
        var blobServiceClient = new BlobServiceClient(blobConnectionString);
        var blobName = "blob-" + Guid.NewGuid();

        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        var blobContainer = containerClient.GetBlobClient(blobName);
        await containerClient.CreateIfNotExistsAsync();
        
        var httpClient = new HttpClient();
        var httpResponse = await httpClient.GetAsync(endpoint);

        var responseStream = httpResponse.IsSuccessStatusCode
            ? await httpResponse.Content.ReadAsStreamAsync()
            : HelperMethods.GenerateStreamFromString("Bad request!");

        await blobContainer.UploadAsync(responseStream, true);
        
        // Handle table stuff
        var tableConnectionString = "UseDevelopmentStorage=true;DevelopmentStorageProxyUri=http://127.0.0.1";
        var tableName = "logs";
        
        var tableClient = new TableClient(tableConnectionString, tableName);
        await tableClient.CreateIfNotExistsAsync();
        
        var newLog = new Log(httpResponse.IsSuccessStatusCode, endpoint, blobName);
        await tableClient.AddEntityAsync(newLog);
    }
}