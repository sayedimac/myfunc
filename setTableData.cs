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

namespace myfunc
{
    public static class setTableData
    {
        [FunctionName("setTableData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            string name = req.Query["name"];
            int qty = Int32.Parse(req.Query["qty"]);
            bool isSale = Boolean.Parse(req.Query["sale"]);
            string rowKey = req.Query["rowkey"];
            string partKey = req.Query["partkey"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            TableClient tableClient = await GetTableClient("tabledata");

            var prod1 = new Product()
            {
                RowKey = rowKey,
                PartitionKey = partKey,
                Name = name,
                Quantity = qty,
                Sale = isSale
            };

            try
            {
                await tableClient.AddEntityAsync<Product>(prod1);
                string responseMessage = "Product: " + name + " added to Table." + " RowKey: " + rowKey + " PartitionKey: " + partKey + " Quantity: " + qty + " Sale: " + isSale;
                return new OkObjectResult(responseMessage);
            }
            catch (System.Exception e)
            {
                string responseMessage = e.Message;
                return new OkObjectResult(responseMessage);
            }
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
