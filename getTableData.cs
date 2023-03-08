using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using Azure;
using System.Text.Json;

namespace myfunc
{
    public static class getTableData
    {
        [FunctionName("getTableData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string rowKey = req.Query["key"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            TableClient tableClient = await GetTableClient("tabledata");

            var product = await tableClient.GetEntityAsync<Product>(
                rowKey: rowKey,
                partitionKey: "website1"
            );
            string responseMessage = JsonSerializer.Serialize(product);
            return new OkObjectResult(responseMessage);
        }

        public static async Task<TableClient> GetTableClient(string theTableName)
        {
            string connstring = Environment.GetEnvironmentVariable("connstring");
            TableServiceClient tableServiceClient = new TableServiceClient(connstring);
            TableClient tableClient = tableServiceClient.GetTableClient(tableName: theTableName);
            await tableClient.CreateIfNotExistsAsync();
            return tableClient;
        }
    }

}
