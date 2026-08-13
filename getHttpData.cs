using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FucntionsApp;

public class getHttpData
{
    private readonly ILogger<getHttpData> _logger;
    private readonly BlobServiceClient _blobServiceClient;

    public getHttpData(ILogger<getHttpData> logger, BlobServiceClient blobServiceClient)
    {
        _logger = logger;
        _blobServiceClient = blobServiceClient;
    }

    [Function("getHttpData")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        // Level 1: BlobServiceClient — represents the storage account
        // (injected via DI, one instance shared across all invocations)

        // Level 2: BlobContainerClient — represents the salesorders container
        var containerClient = _blobServiceClient.GetBlobContainerClient("salesorders");

        // CreateIfNotExists — shows the cost of checking/creating on every request
        await containerClient.CreateIfNotExistsAsync();

        // Enumerate all blobs in the container
        var blobs = new List<string>();
        await foreach (var blobItem in containerClient.GetBlobsAsync())
        {
            // Level 3: BlobClient — represents an individual blob
            var blobClient = containerClient.GetBlobClient(blobItem.Name);

            blobs.Add(blobClient.Uri.ToString());
        }

        return new OkObjectResult(blobs);
    }
}
