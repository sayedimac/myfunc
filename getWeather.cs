using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using myfunc.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace myfunc
{
    public static class getWeather
    {

        private static readonly string[] Summaries = new[]
{
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [FunctionName("getWeather")]
        public static async Task<WeatherForecast[]> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Sent Weather Data");

            var randomNumber = new Random(-20, 55);
            var summaryId = randomNumber.Next(Summaries.Length);
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = randomNumber.Next(-20, 55),
                Summary = Summaries[summaryId]
            })
            .ToArray();


            // String responseMessage =  "[" +  
            //     "{\"date\": \"2022-01-06\",\"temperatureC\": 1, \"summary\": \"Freezing\"}," +
            //     "{\"date\": \"2022-01-07\",\"temperatureC\": 14,\"summary\": \"Bracing\"}," +
            //     "{\"date\": \"2022-01-08\",\"temperatureC\": -13,\"summary\": \"Freezing\"}," +
            //     "{\"date\": \"2022-01-09\",\"temperatureC\": -16,\"summary\": \"Balmy\"}," +
            //     "{\"date\": \"2022-01-10\",\"temperatureC\": -2,\"summary\": \"Chilly\"}" +
            // "]";
            // return new OkObjectResult(responseMessage);
        }
    }
}
