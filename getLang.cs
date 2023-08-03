using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net;
using System.Net.Http.Json;
using System.Net.Http;

namespace myfunc
{
    public static class getLang
    {
        private static readonly string region = Environment.GetEnvironmentVariable("apiloc");
        private static readonly string resourceKey = Environment.GetEnvironmentVariable("apikey");
        private static readonly string endpoint = Environment.GetEnvironmentVariable("apiurl");
        public static string theLang;


        [FunctionName("getLang")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            if (null == region)
            {
                throw new Exception("Please set/export the environment variable: " + region);
            }
            if (null == resourceKey)
            {
                throw new Exception("Please set/export the environment variable: " + resourceKey);
            }
            if (null == endpoint)
            {
                throw new Exception("Please set/export the environment variable: " + endpoint);
            }

            string to = req.Query["to"];
            string txt = req.Query["txt"];
            string route = "/detect?api-version=3.0";
            string detectSentenceText = txt;
            await DetectTextRequest(resourceKey, endpoint, route, detectSentenceText);

            //dynamic data = JsonConvert.DeserializeObject(requestBody);



            return new OkObjectResult(theLang);
        }


        static public async Task DetectTextRequest(string resourceKey, string endpoint, string route, string inputText)
        {
            object[] body = new object[] { new { Text = inputText } };
            var requestBody = JsonSerializer.Serialize(body);
            // var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // Build the request.
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(endpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", resourceKey);
                request.Headers.Add("Ocp-Apim-Subscription-Region", region);

                // Send the request and get response.
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                // Read response as a string.
                string result = await response.Content.ReadAsStringAsync();

                DetectResult[] deserializedOutput = JsonSerializer.Deserialize<DetectResult[]>(result);  // DeserializeObject<DetectResult[]>(result);
                //Iterate through the response.
                // foreach (DetectResult o in deserializedOutput)
                // {
                //     theLang = @"The detected language is '" + o.Language + "'. Confidence is: " + o.Score + ".Translation supported: " + o.IsTranslationSupported + ". Transliteration supported: " + o.IsTransliterationSupported;
                //     //int counter = 0;
                //     // Iterate through alternatives. Use counter for alternative number.
                //     // if (o.Alternatives != null)
                //     // {
                //     //     foreach (AltTranslations a in o.Alternatives)
                //     //     {
                //     //         counter++;
                //     //         // getLang += "Alternative {0}", counter;
                //     //         getLang += "The detected language is '{0}'. Confidence is: {1}.\nTranslation supported: {2}.\nTransliteration supported: {3}.\n", a.Language, a.Score, a.IsTranslationSupported, a.IsTransliterationSupported;
                //     //     }
                //     // }
                // }
            }
        }

    }
    public class DetectResult
    {
        public string Language { get; set; }
        public float Score { get; set; }
        public bool IsTranslationSupported { get; set; }
        public bool IsTransliterationSupported { get; set; }
        public AltTranslations[] Alternatives { get; set; }
    }
    public class AltTranslations
    {
        public string Language { get; set; }
        public float Score { get; set; }
        public bool IsTranslationSupported { get; set; }
        public bool IsTransliterationSupported { get; set; }
    }
}
