using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using System.Text.Json;

namespace myfunc
{
    public static class queryTableData
    {
        [FunctionName("queryTableData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string myPartKey = req.Query["p"];
            string myRowKey = req.Query["r"];
            TableClient tableClient = await GetTableClient("salesorders");
            try
            {
                if (string.IsNullOrEmpty(myRowKey))
                {
                    var products = tableClient.Query<Product>(x => x.PartitionKey == myPartKey);
                    string responseMessage = JsonSerializer.Serialize(products);
                    return new OkObjectResult(responseMessage);
                }
                else
                {
                    var products = tableClient.Query<Product>(x => x.PartitionKey == myPartKey && x.RowKey == myRowKey);
                    string responseMessage = JsonSerializer.Serialize(products);
                    return new OkObjectResult(responseMessage);
                }
            }
            catch (System.Exception e)
            {
                string responseMessage = e.Message;
                return new OkObjectResult(responseMessage);
            }

        }

        public static async Task<TableClient> GetTableClient(string theTableName)
        {
            string connstring = Environment.GetEnvironmentVariable("dbconn");
            TableServiceClient tableServiceClient = new TableServiceClient(connstring);
            TableClient tableClient = tableServiceClient.GetTableClient(tableName: theTableName);
            await tableClient.CreateIfNotExistsAsync();
            return tableClient;
        }
    }

}
