using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MyFunc
{
    public class ProcessSalesOrderFunction
    {
        private readonly ILogger<ProcessSalesOrderFunction> _logger;

        public ProcessSalesOrderFunction(ILogger<ProcessSalesOrderFunction> logger)
        {
            _logger = logger;
        }

        [Function("ProcessSalesOrder")]
        [TableOutput("%HTTP-TRIGGER-TABLE-NAME%", Connection = "TABLE-CONN")]
        public SalesOrderEntity Run([QueueTrigger("salesorders", Connection = "TABLE-CONN")] string queueMessage)
        {
            _logger.LogInformation("Processing sales order: {Message}", queueMessage);

            return new SalesOrderEntity
            {
                PartitionKey = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                RowKey = $"{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}",
                OriginalMessage = queueMessage,
                ProcessedAt = DateTime.UtcNow
            };
        }
    }
}
