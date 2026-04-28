# GitHub Copilot Instructions — FunctionsApp

## Project Overview

This is an **Azure Functions v4 isolated worker** app targeting **.NET 10**. All functions are in the `FucntionsApp` namespace. The worker model uses constructor-injected `ILogger<T>` — never use static loggers or `ILogger` without a type parameter.

## Build

```bash
dotnet build
```

- Target framework is `net10.0`.
- The project uses `Microsoft.Azure.Functions.Worker.Sdk` 2.0.7+ which is required for .NET 10 support.
- If a build error says "Invalid combination of TargetFramework and AzureFunctionsVersion", the SDK package needs updating: `dotnet add package Microsoft.Azure.Functions.Worker.Sdk`.

## Run & Debug Locally

```bash
func start
```

- Requires Azure Functions Core Tools v4.
- Settings must be present in `local.settings.json` at the project root (never commit this file).
- Required keys: `AzureWebJobsStorage`, `TABLE-CONN`, `FUNCTIONS_WORKER_RUNTIME` (value: `dotnet-isolated`).
- HTTP functions are available at `http://localhost:7071/api/<FunctionName>`.
- To debug in VS Code, use the **Attach to .NET Functions** launch configuration.

## Testing

```bash
dotnet test
```

- Unit test function classes by instantiating them directly with a mock `ILogger<T>` (e.g. using `Microsoft.Extensions.Logging.Abstractions.NullLogger<T>`).
- For queue-triggered functions, construct a `QueueMessage` using `QueuesModelFactory.QueueMessage(...)`.
- For HTTP-triggered functions, use `Microsoft.AspNetCore.Http.DefaultHttpContext` to build a mock `HttpRequest`.

## Adding New Functions

Follow these rules when generating new function code:

1. **HTTP triggers must always use `AuthorizationLevel.Anonymous`** — never `AuthorizationLevel.Function` or `AuthorizationLevel.Admin`.
2. Place each function in its own `.cs` file in the project root.
3. Use the isolated worker binding attributes from `Microsoft.Azure.Functions.Worker` (not the in-process `Microsoft.Azure.WebJobs` namespace).
4. Inject `ILogger<T>` via constructor, not via method parameter.
5. Connection string names (e.g. `TABLE-CONN`) must match keys defined in `local.settings.json` and Azure Function App configuration.

### Binding expression rules — CRITICAL

When using binding expressions (e.g. `{token}`) in output binding paths such as `[BlobOutput("container/{token}.txt")]`, only use tokens that are valid for the trigger type. An invalid token causes the function to silently fail to register at startup — it will not appear in logs and will never trigger.

**Valid QueueTrigger binding expression tokens:**

| Token               | Value                                                 |
| ------------------- | ----------------------------------------------------- |
| `{id}`              | Queue message ID — use this for unique blob filenames |
| `{dequeueCount}`    | Number of times the message has been dequeued         |
| `{insertionTime}`   | When the message was added to the queue               |
| `{expirationTime}`  | When the message expires                              |
| `{nextVisibleTime}` | Next visibility time                                  |
| `{popReceipt}`      | Message pop receipt                                   |
| `{queueTrigger}`    | The raw message body text                             |

**Never invent token names** (e.g. `{messageId}`, `{messageText}`) — they will not resolve and the function will not register.

### Blob container prerequisite

The Functions runtime **does not auto-create blob containers**. Before deploying a function with a `[BlobOutput("container/...")]` binding, confirm the container exists:

```bash
az storage container show --name <container> --account-name <storage-account> --auth-mode login
```

If it doesn't exist, create it:

```bash
az storage container create --name <container> --account-name <storage-account> --auth-mode login
```

Queues and tables are auto-created by the runtime; blob containers are not.

### HTTP function template

```csharp
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FucntionsApp;

public class MyFunction
{
    private readonly ILogger<MyFunction> _logger;

    public MyFunction(ILogger<MyFunction> logger)
    {
        _logger = logger;
    }

    [Function("MyFunction")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("MyFunction processed a request.");
        return new OkObjectResult("result");
    }
}
```

### Queue → Table function template

```csharp
using Azure.Data.Tables;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FucntionsApp;

public class MyQueueFunction
{
    private readonly ILogger<MyQueueFunction> _logger;

    public MyQueueFunction(ILogger<MyQueueFunction> logger)
    {
        _logger = logger;
    }

    [Function(nameof(MyQueueFunction))]
    [TableOutput("<table-name>", Connection = "TABLE-CONN")]
    public TableEntity Run([QueueTrigger("<queue-name>", Connection = "TABLE-CONN")] QueueMessage message)
    {
        _logger.LogInformation("Processing message: {messageText}", message.MessageText);

        return new TableEntity("<PartitionKey>", message.MessageId)
        {
            { "MessageText", message.MessageText },
            { "ProcessedAt", DateTime.UtcNow.ToString("o") }
        };
    }
}
```

## Deployment

### Publish locally

```bash
dotnet publish --configuration Release
```

### Deploy via Core Tools

```bash
func azure functionapp publish <function-app-name>
```

### Deploy via Azure CLI (zip deploy)

```bash
az functionapp deployment source config-zip \
  --resource-group <resource-group> \
  --name <function-app-name> \
  --src <path-to-zip>
```

### Required App Settings in Azure

After deploying, ensure the following Application Settings are configured in the Function App:

| Key                        | Value                                |
| -------------------------- | ------------------------------------ |
| `TABLE-CONN`               | Full Azure Storage connection string |
| `FUNCTIONS_WORKER_RUNTIME` | `dotnet-isolated`                    |

## Package Updates

When adding or updating NuGet packages for new trigger/binding types:

```bash
dotnet add package Microsoft.Azure.Functions.Worker.Extensions.<Type>
```

After adding packages, run `dotnet build` to verify no version conflicts exist. If a vulnerability warning appears for a transitive dependency, pin it explicitly:

```bash
dotnet add package <PackageName>
```
