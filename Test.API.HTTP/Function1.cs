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
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;

namespace Test.API.HTTP
{
    public static class Function1
    {

        [FunctionName("GetAllItems")]
        public static IActionResult GetAllItems(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "item")] HttpRequest req,
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
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "item/{id}")] HttpRequest req,
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

        [FunctionName("CreateItem")]
        public static IActionResult CreateItem(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "item")] HttpRequest req,
            [CosmosDB(
                databaseName: "tutorial",
                collectionName: "container1",
                ConnectionStringSetting = "f39-cosmos-tutorial-string")]out dynamic document,
            ILogger log)
        {
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            document = new { itemName = data.itemName, itemPrice = data.itemPrice, itemStock = data.itemStock, partitionKey = "Test"};
            return new OkResult();
        }

        [FunctionName("DeleteItem")]
        public static async Task<IActionResult> DeleteItem(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "item/{id}")] HttpRequest req,
            [CosmosDB(
                databaseName: "tutorial",
                collectionName: "container1",
                ConnectionStringSetting = "f39-cosmos-tutorial-string")] DocumentClient client,
            ILogger log, string id)
        {
            var documentUri = UriFactory.CreateDocumentUri("tutorial", "container1", id);
            var pk = new PartitionKey("Test");
            var options = new RequestOptions { PartitionKey = pk };
            await client.DeleteDocumentAsync(documentUri, options);
            return new OkResult();
        }

        [FunctionName("UpdateItem")]
        public static async Task<IActionResult> UpdateItem(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "item/{id}")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "f39-cosmos-tutorial-string")] DocumentClient client,
            ILogger log, string id)
        {
            var documentUri = UriFactory.CreateDocumentCollectionUri("tutorial", "container1");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            await client.UpsertDocumentAsync(documentUri, new Item { 
                Id = id, 
                ItemName = data.itemName, 
                ItemPrice = data.itemPrice, 
                ItemStock = data.itemStock, 
                PartitionKey = data.partitionKey 
            });
            return new OkResult();
        }
    }
}
