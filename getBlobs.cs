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
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

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

            string container = req.Query["container"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            container = container ?? data?.container;

            // string responseMessage = string.IsNullOrEmpty(container)
            //     ? "Container missing..."
            //     : $"Hello, {container}." + connstring;

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
            string connstring = Environment.GetEnvironmentVariable("connstring");
            BlobServiceClient serviceClient = new BlobServiceClient(connstring);
            BlobContainerClient containerClient = serviceClient.GetBlobContainerClient("images");
            await containerClient.CreateIfNotExistsAsync();
            return containerClient;
        }

        [FunctionName("getHttp")]
        public static async Task<IActionResult> getHttp(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            string container = req.Query["container"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            container = container ?? data?.container;

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            var stringTask = client.GetStringAsync("https://api.github.com/orgs/dotnet/repos");

            var msg = await stringTask;
            return new OkObjectResult(msg);
        }

    public class BlobObject
    {
        public BlobObject(string name, string url)
        {
            blobName = name;
            blobUrl = url;
        }
        public BlobObject()
        {

        }
        public string blobName { get; set; }

        public string blobUrl { get; set; }
    }
        public class Repository
        {
            public string name { get; set; }
        }
}
}