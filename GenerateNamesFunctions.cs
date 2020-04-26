using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Farrellsoft.Examples
{
    public static class GenerateNamesFunctions
    {
        [FunctionName("GenerateNamesTimerFunction")]
        //[return: Queue("names-queue", Connection = "AzureWebJobsStorage")]
        public static async Task<string> RunTrigger(
            [TimerTrigger("*/3 * * * * *")]TimerInfo myTimer,
            ILogger log
        )
        {
            //log.LogInformation("GeneratedNames starting");

            var names = await GetNames();
            //log.LogTrace($"Generated {names.Count} names");

            var output = (new JObject(
                new JProperty("id", Guid.NewGuid().ToString()),
                new JProperty("data", names.Select(name => new JObject(
                    new JProperty("name", name)
                )))
            )).ToString();

            return output;
        }

        static async Task<IList<string>> GetNames()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://www.namegeneratorfun.com");

                var responseString = await client.GetAsync(GetRequestPath());
                var jObject = JObject.Parse(await responseString.Content.ReadAsStringAsync());

                return ((JArray)jObject["names"]).Select(x => x.Value<string>()).ToList();
            }
        }

        static string GetRequestPath()
        {
            var queryParamsHash = new Dictionary<string, string>
            {
                { "generatorType", "list" },
                { "minLength", "0" },
                { "maxLength", "255" },
                { "sexId", "1" },
                { "generatorId", "176" }
            };

            var sb = new StringBuilder("api/namegenerator?");
            var isFirst = true;
            foreach (var kv in queryParamsHash)
            {
                if (!isFirst)
                    sb.Append("&");
                sb.Append($"{kv.Key}={kv.Value}");
                isFirst = false;
            }
            
            return sb.ToString();
        }
    }
}
