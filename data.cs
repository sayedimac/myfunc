using Azure.Data.Tables;
using Azure;
using System;

namespace myfunc
{
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
    public class BlobObject
    {
        public BlobObject(string name, string url)
        {
            blobName = name;
            blobUrl = url;
        }
        public string blobName { get; set; }
        public string blobUrl { get; set; }
    }
}