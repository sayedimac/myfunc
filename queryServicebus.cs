using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using System.Collections.Generic;

namespace myfunc
{
    public static class queryServicebus
    {
        [FunctionName("queryServicebus")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            await using var client = new ServiceBusClient("Endpoint=sb://alwayson.servicebus.windows.net/;SharedAccessKeyName=all;SharedAccessKey=ulrvHtKMSJlP6Mh5jlcJk7qnfaCnLAYPw+ASbJ0Dx60=;EntityPath=salesorders");
            var clientOptions = new ServiceBusClientOptions()
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };
            ServiceBusReceiver receiver = client.CreateReceiver("salesorders");
            IReadOnlyList<ServiceBusReceivedMessage> receivedMessages = await receiver.ReceiveMessagesAsync(maxMessages: 2);
            string body = "";
            foreach (ServiceBusReceivedMessage receivedMessage in receivedMessages)
            {
                body += receivedMessage.Body.ToString();
            }
            return new OkObjectResult(body);
            // try
            // {
            //     processor.ProcessMessageAsync += MessageHandler;
            //     await processor.StartProcessingAsync();
            //     await processor.StopProcessingAsync();
            //     return new OkObjectResult("Done");
            // }
            // catch (System.Exception e)
            // {
            //     string responseMessage = e.Message;
            //     return new OkObjectResult(responseMessage);
            // }
            // finally
            // {
            //     await processor.DisposeAsync();
            //     await client.DisposeAsync();
            // }
        }
        static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            await args.CompleteMessageAsync(args.Message);
        }
    }
}
