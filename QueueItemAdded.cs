using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace myFunc
{
    public class QueueItemAdded
    {
        [FunctionName("QueueItemAdded")]
        [return: Table("salesorders", Connection = "AzureWebJobsStorage")]
        public MyPoco Run([QueueTrigger("salesorders", Connection = "AzureWebJobsStorage")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            return new MyPoco { PartitionKey = "salesorders", RowKey = Guid.NewGuid().ToString(), Text = myQueueItem };
    }
        }
                public class MyPoco
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Text { get; set; }
    }
    }

