using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using System.Text.Json;

namespace MyFunc
{
    public class ListSalesOrdersFunction
    {
        private readonly ILogger _logger;
        private readonly TableServiceClient _tableServiceClient;

        public ListSalesOrdersFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ListSalesOrdersFunction>();
            // Use connection string from environment variable 'TABLE-CONN'
            string connectionString = System.Environment.GetEnvironmentVariable("TABLE-CONN") ?? "";
            _tableServiceClient = new TableServiceClient(connectionString);
        }
        [Function("ListSalesOrders")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "salesorders")] HttpRequestData req)
        {
            var response = req.CreateResponse();
            string tableName = System.Environment.GetEnvironmentVariable("HTTP-TRIGGER-TABLE-NAME") ?? "salesorders";
            var tableClient = _tableServiceClient.GetTableClient(tableName);
            var items = new List<object>();

            await foreach (var entity in tableClient.QueryAsync<TableEntity>())
            {
                items.Add(new
                {
                    PartitionKey = entity.PartitionKey,
                    RowKey = entity.RowKey,
                    Properties = entity.ToDictionary()
                });
            }

            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(JsonSerializer.Serialize(items));
            return response;
        }
    }
}
