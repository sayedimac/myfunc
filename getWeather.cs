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
        [FunctionName("getWeather")]
        public static async Task<WeatherForecast[]> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            List<WeatherForecast> WeatherForecasts = new List<WeatherForecast>();
            //log.LogInformation("Sent Weather Data");
            var randomNumber = new Random();
            int i = 5;
            int temp = 0;


            for (int index = 1; index <= i; index++)
            {
                WeatherForecasts.Add(new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = temp = randomNumber.Next(-20, 55),
                    Summary = GetSummary(temp)
                });
            }
            return WeatherForecasts.ToArray();


            // var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            // {
            //     Date = DateTime.Now.AddDays(index),
            //     TemperatureC = temp = randomNumber.Next(-20, 55),
            //     Summary = GetSummary(temp)
            // }).ToArray();


            // String responseMessage =  "[" +  
            //     "{\"date\": \"2022-01-06\",\"temperatureC\": 1, \"summary\": \"Freezing\"}," +
            //     "{\"date\": \"2022-01-07\",\"temperatureC\": 14,\"summary\": \"Bracing\"}," +
            //     "{\"date\": \"2022-01-08\",\"temperatureC\": -13,\"summary\": \"Freezing\"}," +
            //     "{\"date\": \"2022-01-09\",\"temperatureC\": -16,\"summary\": \"Balmy\"}," +
            //     "{\"date\": \"2022-01-10\",\"temperatureC\": -2,\"summary\": \"Chilly\"}" +
            // "]";
            // return new OkObjectResult(responseMessage);
        }
        private static string GetSummary(int temp)
        {
            var summary = "Mild";

            if (temp >= 32)
            {
                summary = "Hot";
            }
            else if (temp <= 16 && temp > 0)
            {
                summary = "Cold";
            }
            else if (temp <= 0)
            {
                summary = "Freezing!";
            }

            return summary;
        }
    }
}
