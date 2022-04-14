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
        [FunctionName("CreateItem")]
        public static async Task<IActionResult> CreateItem(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "item")] HttpRequest request,
            [CosmosDB(ConnectionStringSetting = "f39-cosmos-tutorial-string")] DocumentClient client,
            ILogger log)
        {
            string requestBody = new StreamReader(request.Body).ReadToEnd();
            dynamic bodyPayload = JsonConvert.DeserializeObject(requestBody);
            Item newItem = new Item
            {
                ItemCategory = bodyPayload.itemCategory,
                ItemName = bodyPayload.itemName,
                ItemPrice = bodyPayload.itemPrice,
                ItemStock = bodyPayload.itemStock
            };
            var itemRepository = new Repository.Repositories.ItemRepository(client);
            var result = await itemRepository.CreateAsync(newItem);
            return new OkObjectResult(result);
        }

        [FunctionName("GetAllItem")]
        public static async Task<IActionResult> GetAllItem(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "item")] HttpRequest request,
            [CosmosDB(ConnectionStringSetting = "f39-cosmos-tutorial-string")] DocumentClient client,
            ILogger log)
        {
            var itemRepository = new Repository.Repositories.ItemRepository(client);
            var result = await itemRepository.GetAsync();
            return new OkObjectResult(result);
        }

        [FunctionName("GetItemById")]
        public static async Task<IActionResult> GetItemById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "item/{id}/{itemCategory}")] HttpRequest request,
            [CosmosDB(ConnectionStringSetting = "f39-cosmos-tutorial-string")] DocumentClient client,
            string id,
            string itemCategory,
            ILogger log)
        {
            var itemRepository = new Repository.Repositories.ItemRepository(client);
            var result = await itemRepository.GetByIdAsync(id, partitionKeys: new Dictionary<string, string>() { { "ItemCategory", itemCategory } });
            return new OkObjectResult(result);
        }

        [FunctionName("DeleteItem")]
        public static async Task<IActionResult> DeleteItem(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "item/{id}/{itemCategory}")] HttpRequest request,
            [CosmosDB(ConnectionStringSetting = "f39-cosmos-tutorial-string")] DocumentClient client,
            string id,
            string itemCategory,
            ILogger log)
        {
            var itemRepository = new Repository.Repositories.ItemRepository(client);
            await itemRepository.DeleteAsync(id, partitionKeys: new Dictionary<string, string>() { { "ItemCategory", itemCategory } });
            return new OkObjectResult("Item Deleted");
        }

        [FunctionName("UpdateItem")]
        public static async Task<IActionResult> UpdateItem(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "item/{id}/{itemCategory}")] HttpRequest request,
            [CosmosDB(ConnectionStringSetting = "f39-cosmos-tutorial-string")] DocumentClient client,
            string id,
            string itemCategory,
            ILogger log)
        {
            string requestBody = new StreamReader(request.Body).ReadToEnd();
            dynamic bodyPayload = JsonConvert.DeserializeObject(requestBody);
            var itemRepository = new Repository.Repositories.ItemRepository(client);
            var documentItem = await itemRepository.GetByIdAsync(id, partitionKeys: new Dictionary<string, string>() { { "ItemCategory", itemCategory } });
            documentItem.ItemName = bodyPayload.itemName;
            documentItem.ItemPrice = bodyPayload.itemPrice;
            documentItem.ItemStock = bodyPayload.itemStock;
            var result = await itemRepository.UpdateAsync(id, documentItem);
            return new OkObjectResult(documentItem);
        }
    }
}
