using Azure;
using Azure.Data.Tables;
using System;

namespace MyFunc
{
    public class SalesOrderEntity : ITableEntity
    {
        public required string PartitionKey { get; set; }
        public required string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string? OriginalMessage { get; set; }
        public DateTime ProcessedAt { get; set; }
        public string? ProcessedBy { get; set; }
        public string? OrderId { get; set; }
        public string? CustomerId { get; set; }
        public decimal? Amount { get; set; }
        public string? Status { get; set; }
    }
}
