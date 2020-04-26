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
            [CosmosDB(databaseName: "namesdata", collectionName: "firstletters",
                ConnectionStringSetting = "CosmosDbConnection")]out dynamic document,
            ILogger log)
        {
            //log.LogInformation("CreateAggregateDocument starting");

            var eventDataContents = req.ReadAsStringAsync().GetAwaiter().GetResult();
            var countsArray = JArray.Parse(eventDataContents);

            // create the collated event data into our document db
            document = new {
                id = Guid.NewGuid(),
                timestamp = DateTime.UtcNow,
                eventData = countsArray
            };

            //log.LogTrace($"Registered document with id {document.id} for creation");

            return new OkObjectResult("received");
        }
    }
}
