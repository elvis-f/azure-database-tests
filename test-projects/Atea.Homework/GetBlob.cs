using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Atea.Homework;

public static class GetBlob
{
    [FunctionName("GetBlob")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ILogger log)
    {
        log.LogInformation(" GET API to fetch a payload from blob for specific log entry");
        
        var blob = HelperMethods.GetParameter(req, "blob");
        var containerName = "my-container";
        
        if (blob == null)
        {
            return new BadRequestObjectResult("Missing blob name!");
        }
        
        var blobConnectionString = "UseDevelopmentStorage=true;DevelopmentStorageProxyUri=http://localhost:10000";
        var blobServiceClient = new BlobServiceClient(blobConnectionString);

        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blob);

        if (!await blobClient.ExistsAsync())
        {
            return new BadRequestObjectResult("Blob not found!");
        }
        
        var incomingStream = new MemoryStream();
        await blobClient.DownloadToAsync(incomingStream);

        var incomingPayload = HelperMethods.ParseStream(incomingStream);

        return new OkObjectResult(incomingPayload);
    }
}