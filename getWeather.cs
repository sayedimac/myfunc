using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
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

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = new Random().Next(-20, 55),
                Summary = getSummary(new Random().Next(0, 10))
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
        public static string getSummary(int id)
        {
            var summaryId = id;
            string? theSummary = "" ?? "No weather at all!";
            switch (summaryId)
            {
                case 0:
                    theSummary = "No weather at all!";
                    break;
                case 9:
                    theSummary = "Extreme weather";
                    break;
                case 1:
                    theSummary = "Semi Extreme weather";
                    break;
                case 2:
                    theSummary = "Less Extreme weather";
                    break;
                case 7:
                    theSummary = "Tough weather";
                    break;
                case 8:
                    theSummary = "Severe weather";
                    break;
                default:
                    theSummary = "Easy peasy weather";
                    break;
            }
            return theSummary;
        }
    }
}
