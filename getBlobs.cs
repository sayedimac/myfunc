using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace myfunc
{
    public static class getBlobs
    {

        [FunctionName("getBlobs")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            string container = req.Query["container"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            container = container ?? data?.container;

            // string responseMessage = string.IsNullOrEmpty(container)
            //     ? "Container missing..."
            //     : $"Hello, {container}." + connstring;

            BlobContainerClient containerClient = await GetCloudBlobContainer(container);
            List<string> results = new List<string>();
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                results.Add(
                    Flurl.Url.Combine(
                        containerClient.Uri.AbsoluteUri,
                        blobItem.Name
                    )
                );
            }
            return new OkObjectResult(results);
        }

        public static async Task<BlobContainerClient> GetCloudBlobContainer(string containerName)
        {
            //string connstring = "DefaultEndpointsProtocol=https;AccountName=alwayson;AccountKey=xybXZHETeC8ms5sLtmuvimPlqTf5tU1493wB7cyEuybLe/V2QciqDo2D2VKjODR0K8vKCbeJ8syA+ASt6XmYAw==;EndpointSuffix=core.windows.net";
            string connstring = Environment.GetEnvironmentVariable("connstring");
            BlobServiceClient serviceClient = new BlobServiceClient(connstring);
            BlobContainerClient containerClient = serviceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            return containerClient;
        }
    }
}
