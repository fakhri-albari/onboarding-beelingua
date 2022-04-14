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

        //[FunctionName("GetAllItems")]
        //public static IActionResult GetAllItems(
        //    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "item")] HttpRequest req,
        //    [CosmosDB(
        //        databaseName: "tutorial",
        //        collectionName: "container1",
        //        ConnectionStringSetting = "f39-cosmos-tutorial-string",
        //        PartitionKey = "Test")] IEnumerable<Item> result,
        //    ILogger log)
        //{
        //    return new OkObjectResult(result);
        //}

        //[FunctionName("GetItemById")]
        //public static IActionResult GetItemById(
        //    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "item/{id}")] HttpRequest req,
        //    [CosmosDB(
        //        databaseName: "tutorial",
        //        collectionName: "container1",
        //        ConnectionStringSetting = "f39-cosmos-tutorial-string",
        //        PartitionKey = "Test",
        //        Id ="{id}")] Item result,
        //    ILogger log)
        //{
        //    return new OkObjectResult(result);
        //}

        //[FunctionName("CreateItem")]
        //public static async Task<IActionResult> CreateItem(
        //    [HttpTrigger(AuthorizationLevel.Function, "post", Route = "item")] HttpRequest req,
        //    [CosmosDB(ConnectionStringSetting = "f39-cosmos-tutorial-string")] DocumentClient client,
        //    ILogger log)
        //{
        //string requestBody = new StreamReader(req.Body).ReadToEnd();
        //dynamic data = JsonConvert.DeserializeObject(requestBody);
        //Item item = new Item
        //{
        //    ItemName = data.itemName,
        //    ItemPrice = data.itemPrice,
        //    ItemStock = data.itemStock,
        //    PartitionKey = "Test"
        //};
        //    var databaseURI = UriFactory.CreateDocumentCollectionUri("tutorial", "container1");
        //    var result = await client.CreateDocumentAsync(databaseURI, item);
        //    return new OkObjectResult("Item Created");
        //}

        //[FunctionName("DeleteItem")]
        //public static async Task<IActionResult> DeleteItem(
        //    [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "item/{id}")] HttpRequest req,
        //    [CosmosDB(
        //        databaseName: "tutorial",
        //        collectionName: "container1",
        //        ConnectionStringSetting = "f39-cosmos-tutorial-string")] DocumentClient client,
        //    ILogger log, string id)
        //{
        //    var documentUri = UriFactory.CreateDocumentUri("tutorial", "container1", id);
        //    var pk = new PartitionKey("Test");
        //    var options = new RequestOptions { PartitionKey = pk };
        //    await client.DeleteDocumentAsync(documentUri, options);
        //    return new OkResult();
        //}

        //[FunctionName("UpdateItem")]
        //public static async Task<IActionResult> UpdateItem(
        //    [HttpTrigger(AuthorizationLevel.Function, "put", Route = "item/{id}")] HttpRequest req,
        //    [CosmosDB(ConnectionStringSetting = "f39-cosmos-tutorial-string")] DocumentClient client,
        //    ILogger log, string id)
        //{
        //    var documentUri = UriFactory.CreateDocumentCollectionUri("tutorial", "container1");
        //    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        //    dynamic data = JsonConvert.DeserializeObject(requestBody);
        //    await client.UpsertDocumentAsync(documentUri, new Item { 
        //        Id = id, 
        //        ItemName = data.itemName, 
        //        ItemPrice = data.itemPrice, 
        //        ItemStock = data.itemStock, 
        //        PartitionKey = data.partitionKey 
        //    });
        //    return new OkResult();
        //}

        [FunctionName("NexusCreateItem")]
        public static async Task<IActionResult> NexusCreateItem(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "item")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "f39-cosmos-tutorial-string")] DocumentClient client,
            ILogger log)
        {
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            Item item = new Item
            {
                ItemName = data.itemName,
                ItemPrice = data.itemPrice,
                ItemStock = data.itemStock
            };
            var classRep = new Repository.Repositories.ItemRepository(client);
            var final = await classRep.CreateAsync(item);
            return new OkObjectResult(final);
        }

        [FunctionName("NexusGetAllItem")]
        public static async Task<IActionResult> NexusGetAllItem(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "item")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "f39-cosmos-tutorial-string")] DocumentClient client,
            ILogger log)
        {
            var classRep = new Repository.Repositories.ItemRepository(client);
            var final = await classRep.GetAsync();
            return new OkObjectResult(final);
        }

        [FunctionName("NexusGetItemById")]
        public static async Task<IActionResult> NexusGetItemById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "item/{id}/{itemName}")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "f39-cosmos-tutorial-string")] DocumentClient client,
            string id,
            string itemName,
            ILogger log)
        {
            var classRep = new Repository.Repositories.ItemRepository(client);
            var final = await classRep.GetByIdAsync(id, partitionKeys: new Dictionary<string, string>() { { "ItemName", itemName } });
            return new OkObjectResult(final);
        }

        [FunctionName("NexusDeleteItem")]
        public static async Task<IActionResult> NexusDeleteItem(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "item/{id}/{itemName}")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "f39-cosmos-tutorial-string")] DocumentClient client,
            string id,
            string itemName,
            ILogger log)
        {
            var classRep = new Repository.Repositories.ItemRepository(client);
            await classRep.DeleteAsync(id, partitionKeys: new Dictionary<string, string>() { { "ItemName", itemName } });
            return new OkObjectResult("item deleted");
        }

        [FunctionName("NexusUpdateItem")]
        public static async Task<IActionResult> NexusUpdateItem(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "item/{id}/{itemName}")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "f39-cosmos-tutorial-string")] DocumentClient client,
            string id,
            string itemName,
            ILogger log)
        {
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            var classRep = new Repository.Repositories.ItemRepository(client);
            var documentItem = await classRep.GetByIdAsync(id, partitionKeys: new Dictionary<string, string>() { { "ItemName", itemName } });
            documentItem.ItemName = data.itemName;
            documentItem.ItemPrice = data.itemPrice;
            documentItem.ItemStock = data.itemStock;
            var final = await classRep.UpdateAsync(id, documentItem);
            return new OkObjectResult("Item Updated");
        }
    }
}
