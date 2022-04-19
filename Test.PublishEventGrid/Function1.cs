using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Messaging.EventGrid;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Test.PublishEventGrid
{
    public static class Function1
    {
        [FunctionName("ItemUpdated")]
        public static async Task ItemUpdated(
            [CosmosDBTrigger(
                databaseName: "tutorial",
                collectionName: "Item",
                ConnectionStringSetting = "f39-cosmos-tutorial-string",
                LeaseCollectionName = "leases",
                CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input,
            [EventGrid(TopicEndpointUri = "EventGridEndpoint", TopicKeySetting = "EventGridKey")] IAsyncCollector<EventGridEvent> eventCollector,
            ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                string message = "Documents modified " + input.Count;

                log.LogInformation(message);

                EventGridEvent e = new EventGridEvent(message, "IncomingRequest", "IncomingRequest", "1.0.0");
                await eventCollector.AddAsync(e);
            }
        }
    }
}
