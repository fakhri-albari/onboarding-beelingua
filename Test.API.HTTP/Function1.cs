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
using Test.API.HTTP.Models;

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
                ItemCategory = bodyPayload.ItemCategory,
                ItemName = bodyPayload.ItemName,
                ItemPrice = bodyPayload.ItemPrice,
                ItemStock = bodyPayload.ItemStock
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
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "item/{id}/{ItemCategory}")] HttpRequest request,
            [CosmosDB(ConnectionStringSetting = "f39-cosmos-tutorial-string")] DocumentClient client,
            string id,
            string ItemCategory,
            ILogger log)
        {
            var itemRepository = new Repository.Repositories.ItemRepository(client);
            var result = await itemRepository.GetByIdAsync(id, partitionKeys: new Dictionary<string, string>() { { "ItemCategory", ItemCategory } });
            return new OkObjectResult(result);
        }

        [FunctionName("DeleteItem")]
        public static async Task<IActionResult> DeleteItem(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "item/{id}/{ItemCategory}")] HttpRequest request,
            [CosmosDB(ConnectionStringSetting = "f39-cosmos-tutorial-string")] DocumentClient client,
            string id,
            string ItemCategory,
            ILogger log)
        {
            var itemRepository = new Repository.Repositories.ItemRepository(client);
            await itemRepository.DeleteAsync(id, partitionKeys: new Dictionary<string, string>() { { "ItemCategory", ItemCategory } });
            return new OkObjectResult("Item Deleted");
        }

        [FunctionName("UpdateItem")]
        public static async Task<IActionResult> UpdateItem(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "item")] HttpRequest request,
            [CosmosDB(ConnectionStringSetting = "f39-cosmos-tutorial-string")] DocumentClient client,
            ILogger log)
        {
            string requestBody = new StreamReader(request.Body).ReadToEnd();
            dynamic bodyPayload = JsonConvert.DeserializeObject(requestBody);
            string ItemCategory = bodyPayload.ItemCategory;
            string itemId = bodyPayload.ItemId;
            var itemRepository = new Repository.Repositories.ItemRepository(client);
            var documentItem = await itemRepository.GetByIdAsync(itemId, partitionKeys: new Dictionary<string, string>() { { "ItemCategory", ItemCategory } });
            documentItem.ItemName = bodyPayload.ItemName;
            documentItem.ItemPrice = bodyPayload.ItemPrice;
            documentItem.ItemStock = bodyPayload.ItemStock;
            var result = await itemRepository.UpdateAsync(itemId, documentItem);
            return new OkObjectResult(documentItem);
        }

        [FunctionName("UpdateItemStock")]
        public static async Task<IActionResult> UpdateItemStock(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "item/stock")] HttpRequest request,
        [CosmosDB(ConnectionStringSetting = "f39-cosmos-tutorial-string")] DocumentClient client,
        ILogger log)
        {
            string requestBody = new StreamReader(request.Body).ReadToEnd();
            dynamic bodyPayload = JsonConvert.DeserializeObject(requestBody);
            var itemRepository = new Repository.Repositories.ItemRepository(client);
            string ItemCategory = bodyPayload.ItemCategory;
            string itemId = bodyPayload.ItemId;
            var documentItem = await itemRepository.GetByIdAsync(itemId, partitionKeys: new Dictionary<string, string>() { { "ItemCategory", ItemCategory } });
            int quantity = bodyPayload.ItemQuantity;
            documentItem.ItemStock = documentItem.ItemStock - quantity;
            var result = await itemRepository.UpdateAsync(itemId, documentItem);
            return new OkObjectResult(documentItem);
        }

        [FunctionName("GetAllOrder")]
        public static async Task<IActionResult> GetAllOrder(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "order")] HttpRequest request,
            [CosmosDB(ConnectionStringSetting = "f39-cosmos-tutorial-string")] DocumentClient client,
            ILogger log)
        {
            var orderRepository = new Repository.Repositories.OrdersRepository(client);
            var result = await orderRepository.GetAsync();
            return new OkObjectResult(result);
        }

        [FunctionName("CreateOrder")]
        public static async Task<IActionResult> CreateOrder(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "order")] HttpRequest request,
        [CosmosDB(ConnectionStringSetting = "f39-cosmos-tutorial-string")] DocumentClient client,
        ILogger log)
        {
            string requestBody = new StreamReader(request.Body).ReadToEnd();
            dynamic bodyPayload = JsonConvert.DeserializeObject(requestBody);
            List<OrderItem> itemList = new List<OrderItem>();
            foreach (dynamic items in bodyPayload.Items) {
                OrderItem item = new OrderItem() 
                { 
                    ItemId = items.ItemId,
                    ItemCategory = items.ItemCategory,
                    ItemName = items.ItemName,
                    ItemPrice = items.ItemPrice,
                    ItemQuantity = items.ItemQuantity
                };
                itemList.Add(item);
            }
            Orders newOrder = new Orders
            {
                OrderCategory = bodyPayload.OrderCategory,
                PaymentMethod = bodyPayload.PaymentMethod,
                TotalQuantity = bodyPayload.TotalQuantity,
                TotalPrice = bodyPayload.TotalPrice,
                Items = itemList
            };
            var ordersRepository = new Repository.Repositories.OrdersRepository(client);
            var result = await ordersRepository.CreateAsync(newOrder);
            return new OkObjectResult(result);
        }

        
    }
}
