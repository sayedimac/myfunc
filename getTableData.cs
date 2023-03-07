using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Data.Tables;
using Azure;

namespace myfunc
{
    public static class getTableData
    {
        [FunctionName("getTableData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];
            int qty = Int32.Parse(req.Query["qty"]);
            bool isSale = Boolean.Parse(req.Query["sale"]);
            string partKey = req.Query["key"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            TableClient tableClient = await GetTableClient(name);

            // Create new item using composite key constructor
            var prod1 = new Product()
            {
                RowKey = "68719518388",
                PartitionKey = partKey,
                Name = name,
                Quantity = qty,
                Sale = isSale
            };

            // Add new item to server-side table
            await tableClient.AddEntityAsync<Product>(prod1);

            // Read a single item from container
            var product = await tableClient.GetEntityAsync<Product>(
                rowKey: "68719518388",
                partitionKey: partKey
            );

            string responseMessage = product.Value.Name;

            return new OkObjectResult(responseMessage);
        }


        public static async Task<TableClient> GetTableClient(string theTableName)
        {
            string connstring = Environment.GetEnvironmentVariable("connstring");
            TableServiceClient tableServiceClient = new TableServiceClient(connstring);
            TableClient tableClient = tableServiceClient.GetTableClient(tableName: theTableName);

            await tableClient.CreateIfNotExistsAsync();

            // string container = Environment.GetEnvironmentVariable("container");
            // BlobServiceClient serviceClient = new BlobServiceClient(connstring);
            // BlobContainerClient containerClient = serviceClient.GetBlobContainerClient(container);
            // await containerClient.CreateIfNotExistsAsync();
            return tableClient;
        }
    }
    // C# record type for items in the table
    public record Product : ITableEntity
    {
        public string RowKey { get; set; } = default!;

        public string PartitionKey { get; set; } = default!;

        public string Name { get; init; } = default!;

        public int Quantity { get; init; }

        public bool Sale { get; init; }

        public ETag ETag { get; set; } = default!;

        public DateTimeOffset? Timestamp { get; set; } = default!;
    }

}
