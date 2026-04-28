using Azure.Data.Tables;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FucntionsApp;

public class QueueAddedOutput
{
    [TableOutput("salesorders", Connection = "TABLE-CONN")]
    public TableEntity? TableEntity { get; set; }

    [BlobOutput("salesorders/{id}.txt", Connection = "TABLE-CONN")]
    public string? BlobContent { get; set; }
}

public class queueAdded
{
    private readonly ILogger<queueAdded> _logger;

    public queueAdded(ILogger<queueAdded> logger)
    {
        _logger = logger;
    }

    [Function(nameof(queueAdded))]
    public QueueAddedOutput Run([QueueTrigger("salesorders", Connection = "TABLE-CONN")] QueueMessage message)
    {
        var messageText = message.Body.ToString();
        _logger.LogInformation("C# Queue trigger function processed: {messageText}", messageText);

        var processedAt = DateTime.UtcNow.ToString("o");

        return new QueueAddedOutput
        {
            BlobContent = $"MessageId: {message.MessageId}\nMessageText: {messageText}\nInsertedOn: {message.InsertedOn?.ToString("o")}\nProcessedAt: {processedAt}",
            TableEntity = new TableEntity("SalesOrders", message.MessageId)
            {
                { "MessageText", messageText },
                { "InsertedOn", message.InsertedOn?.ToString("o") },
                { "ProcessedAt", processedAt }
            }
        };
    }
}