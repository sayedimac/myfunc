# The myfunc app is a C# Function App that runs in Azure

[![Build and deploy dotnet core project to Azure Function App - alwayson](https://github.com/sayedimac/myfunc/actions/workflows/main_alwayson.yml/badge.svg)](https://github.com/sayedimac/myfunc/actions/workflows/main_alwayson.yml)

## Simple Azure function that reads blobs and table data from an Azure storage account. 
You can publish this app to an Azure Function app by following the steps below:

1. Create an Azure Function App  - this should also create a storage account that will manage the state of Durable Functions should you want to make use of those
2. Navigate to the Storage account and copy the connection string from *Security & Networking / Keys* - you will use this next when you configure the Function App
3. Create Blob Container in the storage account called *images* and upload some images in there so there is some sample data  
4. Go back to the function app, select *Configuration* under *Settings*
5. Add a setting by Clicking on the *+ New application setting*, add a setting named *connstring* and make the value of this the connection string from the earlier step
6. Click on the *+ New application setting* again and add a setting named *container* and make the value *images*
7. You can now publish this app to the function app by either making use of the deployment center (this will create a Github Action file that will essentially use CI/CD) or you simply publish the app from VScode or even from the commandline (Azure CLI) or the Azure functions Core tools command line tools   


