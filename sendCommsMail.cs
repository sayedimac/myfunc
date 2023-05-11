using System;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using Azure;
using Azure.Communication.Email;

namespace myfunc
{
    public static class sendCommsMail
    {
        [FunctionName("sendCommsMail")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string subject = req.Query["subject"];
            string message = req.Query["htmlContent"];
            string sender = "donotreply@99bdb9b8-88ab-4808-931d-e2f2c537caa3.azurecomm.net";
            string recipient = req.Query["recipient"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonSerializer.Deserialize(requestBody, typeof(string));
            subject = subject ?? data?.subject;
            message = message ?? data?.message;
            recipient = recipient ?? data?.recipient;

            List<string> list = new List<string> { subject, message, recipient };
            string responseMessage = ""; ;
            foreach (string s in list)
            {
                if (string.IsNullOrEmpty(s))
                {
                    responseMessage = "Please pass a subject, message and recipient in the query string or in the request body";
                }
                else
                {
                    string connectionString = Environment.GetEnvironmentVariable("commsconnectionstring");
                    EmailClient client = new EmailClient(connectionString);
                    try
                    {
                        EmailSendOperation emailSendOperation = await client.SendAsync(
                            Azure.WaitUntil.Completed,
                            sender,
                            recipient,
                            subject,
                            message);
                        EmailSendResult statusMonitor = emailSendOperation.Value;

                        responseMessage = $"Email Sent. Status = {emailSendOperation.Value.Status} with id: {emailSendOperation.Id}";
                    }
                    catch (RequestFailedException ex)
                    {
                        /// OperationID is contained in the exception message and can be used for troubleshooting purposes
                        responseMessage = $"Email send operation failed with error code: {ex.ErrorCode}, message: {ex.Message}";
                    }
                }
            }
            return new OkObjectResult(responseMessage);
        }
    }
}

