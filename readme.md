# FunctionsApp

Azure Functions v4 isolated worker app targeting **.NET 10**, using queue and HTTP triggers with Azure Table Storage output.

## Functions

| Function | Trigger | Description |
|---|---|---|
| `getHttpData` | HTTP (Anonymous) | Responds to GET/POST requests |
| `queueAdded` | Queue (`salesorders`) | Reads messages from the `salesorders` queue and writes them to the `salesorders` Azure Table |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Azure Functions Core Tools v4](https://learn.microsoft.com/azure/azure-functions/functions-run-local)
- [Azurite](https://learn.microsoft.com/azure/storage/common/storage-use-azurite) or an Azure Storage account

## Configuration

Settings are stored in `local.settings.json` (not committed to source control).

| Setting | Description |
|---|---|
| `AzureWebJobsStorage` | Storage connection string used by the Functions runtime |
| `TABLE-CONN` | Connection string for the `salesorders` queue and table |
| `FUNCTIONS_WORKER_RUNTIME` | Must be `dotnet-isolated` |

Example `local.settings.json`:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "<your-connection-string>",
    "TABLE-CONN": "<your-connection-string>",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
  }
}
```

## Build

```bash
dotnet build
```

## Run locally

```bash
func start
```

The HTTP function will be available at `http://localhost:7071/api/getHttpData`.

To trigger `queueAdded`, add a message to the `salesorders` queue in the storage account referenced by `TABLE-CONN`.

## Test

```bash
dotnet test
```

To manually test the HTTP trigger:

```bash
curl http://localhost:7071/api/getHttpData
```

To manually test the queue trigger, use [Azure Storage Explorer](https://azure.microsoft.com/products/storage/storage-explorer/) or the Azure portal to add a message to the `salesorders` queue.

## Publish

```bash
dotnet publish --configuration Release
```

## Deploy to Azure

```bash
func azure functionapp publish <your-function-app-name>
```

Or with the Azure CLI:

```bash
az functionapp deployment source config-zip \
  --resource-group <rg> \
  --name <function-app-name> \
  --src bin/publish/<app>.zip
```

## Key Packages

| Package | Version |
|---|---|
| `Microsoft.Azure.Functions.Worker` | 2.52.0 |
| `Microsoft.Azure.Functions.Worker.Sdk` | 2.0.7 |
| `Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore` | 2.1.0 |
| `Microsoft.Azure.Functions.Worker.Extensions.Storage.Queues` | 5.5.4 |
| `Microsoft.Azure.Functions.Worker.Extensions.Tables` | 1.5.0 |


hahaha
