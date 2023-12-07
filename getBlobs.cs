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
using System.Net.Http;

namespace myfunc
{
    public static class getBlobs
    {
        private static readonly HttpClient client = new HttpClient();


        [FunctionName("getBlobs")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            string container = "images" //req.Query["container"];

            // string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            // dynamic data = JsonConvert.DeserializeObject(requestBody);
            // container = container ?? data?.container;

            BlobContainerClient containerClient = await GetCloudBlobContainer(container);
            List<BlobObject> results = new List<BlobObject>();
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                BlobObject blobObject = new BlobObject(blobItem.Name, Flurl.Url.Combine(
                        containerClient.Uri.AbsoluteUri,
                        blobItem.Name
                    ));
                results.Add(blobObject);
            }
            return new OkObjectResult(results);
        }

        public static async Task<BlobContainerClient> GetCloudBlobContainer(string containerName)
        {
            string connstring = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string container =  Environment.GetEnvironmentVariable("container");
            BlobServiceClient serviceClient = new BlobServiceClient(connstring);
            BlobContainerClient containerClient = serviceClient.GetBlobContainerClient(container.ToLower());
            await containerClient.CreateIfNotExistsAsync();
            return containerClient;
        }
    }
}
