using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Test.API.HTTP
{
    public static class Function1
    {

        [FunctionName("GetAllItems")]
        public static IActionResult GetAllItems(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Item")] HttpRequest req,
            [CosmosDB(
                databaseName: "tutorial",
                collectionName: "container1",
                ConnectionStringSetting = "f39-cosmos-tutorial-string",
                PartitionKey = "Test")] IEnumerable<Item> result,
            ILogger log)
        {
            return new OkObjectResult(result);
        }

        [FunctionName("GetItemById")]
        public static IActionResult GetItemById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Item/{id}")] HttpRequest req,
            [CosmosDB(
                databaseName: "tutorial",
                collectionName: "container1",
                ConnectionStringSetting = "f39-cosmos-tutorial-string",
                PartitionKey = "Test",
                Id ="{id}")] Item result,
            ILogger log)
        {
            return new OkObjectResult(result);
        }
    }
}
