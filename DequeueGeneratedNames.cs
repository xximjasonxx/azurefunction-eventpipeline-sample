using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Farrellsoft.Examples
{
    public static class DequeueGeneratedNames
    {
        [FunctionName("DequeueGeneratedNames")]
        [return: Blob("raw-names/{id}.txt", FileAccess.Write, Connection = "AzureWebJobsStorage")]
        public static string Run(
            [QueueTrigger("names-queue", Connection = "AzureWebJobsStorage")]string myQueueItem,
            [EventHub("names", Connection = "EventHubSendConnection")]IAsyncCollector<string> outputEvents,
            ILogger log)
        {
            //log.LogTrace($"Dequeued {myQueueItem} from Queue");

            ((JArray)JObject.Parse(myQueueItem)["data"]).ToList()
                .ForEach((obj) =>
                {
                    //log.LogTrace($"Hello {obj["name"].Value<string>()}");
                    outputEvents.AddAsync(obj.ToString());
                });
            
            return myQueueItem;
        }
    }
}
