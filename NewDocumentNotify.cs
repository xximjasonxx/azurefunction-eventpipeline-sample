using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;

namespace Farrellsoft.Examples
{
    public static class NewDocumentNotify
    {
        [FunctionName("NewDocumentNotify")]
        public static async Task Run(
            [CosmosDBTrigger(databaseName: "namesdata", collectionName: "firstletters",
                ConnectionStringSetting = "CosmosDbConnection",
                CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> newDocuments,
            [SignalR(HubName = "FirstNameLetter")]IAsyncCollector<SignalRMessage> signalRMessage,
            ILogger log)
        {
            log.LogInformation($"Sending {newDocuments.Count} documents");
            await signalRMessage.AddAsync(new SignalRMessage
                {
                    Target = "newFirstLetterData",
                    Arguments = newDocuments.ToArray()
                });
        }

        [FunctionName("negotiate")]
        public static SignalRConnectionInfo GetSignalRInfo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            [SignalRConnectionInfo(HubName = "FirstNameLetter")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }
    }
}
