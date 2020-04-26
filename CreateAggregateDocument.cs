using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Farrellsoft.Examples
{
    public static class CreateAggregateDocument
    {
        [FunctionName("CreateAggregateDocument")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [CosmosDB(databaseName: "namesdata", collectionName: "firstletterstrend",
                ConnectionStringSetting = "CosmosDbConnection")]out dynamic document,
            ILogger log)
        {
            //log.LogInformation("CreateAggregateDocument starting");

            var eventDataContents = req.ReadAsStringAsync().GetAwaiter().GetResult();
            
            // create the collated event data into our document db
            document = new {
                id = Guid.NewGuid(),
                timestamp = DateTime.UtcNow,
                eventData = JArray.Parse(eventDataContents)
            };

            //log.LogTrace($"Registered document with id {document.id} for creation");

            return new OkObjectResult("received");
        }
    }
}
