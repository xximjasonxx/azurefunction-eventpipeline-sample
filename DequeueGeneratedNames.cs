using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Farrellsoft.Examples
{
    public static class DequeueGeneratedNames
    {
        [FunctionName("DequeueGeneratedNames")]
        [return: Blob("raw-names/{id}.txt", FileAccess.Write, Connection = "AzureWebJobsStorage")]
        public static string Run(
            //[QueueTrigger("names-queue", Connection = "AzureWebJobsStorage")]string myQueueItem,
            [ServiceBusTrigger("newnames-queue", Connection = "ServiceBusConnection")]NamesGenerationRecord msg,
            [EventHub("names", Connection = "EventHubSendConnection")]IAsyncCollector<string> outputEvents,
            ILogger log)
        {
            //log.LogTrace($"Dequeued {myQueueItem} from Queue");

            foreach (var nameRecord in msg.GeneratedNames)
            {
                //log.LogTrace($"Hello {obj["name"].Value<string>()}");
                outputEvents.AddAsync(nameRecord.ToString());
            }
            
            return msg.ToString();
        }
    }

    public class NamesGenerationRecord
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("data")]
        public IList<JObject> GeneratedNames {get; set; }

        public override string ToString()
        {
            var data = new JObject(
                new JProperty("id", Id),
                new JProperty("data", GeneratedNames.Select(x => new JObject(
                    new JProperty("name", x)
                )))
            );

            return data.ToString();
        }
    }
}
