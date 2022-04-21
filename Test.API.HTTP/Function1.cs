using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Test.DAL.Models;
using Test.DAL.Repository;
using System.Net;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using Test.BLL;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Test.API.HTTP
{
    public class Function1
    {
        private readonly CosmosClient _client;

        public Function1(CosmosClient client)
        {
            _client = client;
        }

        [FunctionName("CreateItem")]
        [OpenApiOperation(operationId: "CreateItem", tags: new[] { "Item" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Item), Description = "Item want to be created", Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Item), Description = "The OK response")]
        public async Task<IActionResult> CreateItem(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "item")] HttpRequest request,
            ILogger log)
        {
            string requestBody = new StreamReader(request.Body).ReadToEnd();
            var itemSvc = new ItemService(new Repositories.ItemRepository(_client));
            var result = await itemSvc.CreateItem(requestBody);
            return new OkObjectResult(result);
        }

        [FunctionName("GetAllItem")]
        [OpenApiOperation(operationId: "GetAllItem", tags: new[] { "Item" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Item), Description = "The OK response")]
        public async Task<IActionResult> GetAllItem(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "item")] HttpRequest request,
            ILogger log)
        {
            var itemSvc = new ItemService(new Repositories.ItemRepository(_client));
            var result = await itemSvc.GetAllItem();
            return new OkObjectResult(result);
        }

        [FunctionName("GetItemById")]
        [OpenApiOperation(operationId: "GetItemById", tags: new[] { "Item" })]
        [OpenApiParameter("id", Type = typeof(string), In = ParameterLocation.Path)]
        [OpenApiParameter("ItemCategory", Type = typeof(string), In = ParameterLocation.Path)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Item), Description = "The OK response")]
        public async Task<IActionResult> GetItemById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "item/{id}/{ItemCategory}")] HttpRequest request,
            string id,
            string ItemCategory,
            ILogger log)
        {
            var itemSvc = new ItemService(new Repositories.ItemRepository(_client));
            var result = await itemSvc.GetByIdAsync(id, new Dictionary<string, string>() { { "ItemCategory", ItemCategory } });
            return new OkObjectResult(result);
        }

        [FunctionName("DeleteItem")]
        [OpenApiOperation(operationId: "DeleteItem", tags: new[] { "Item" })]
        [OpenApiParameter("id", Type = typeof(string), In = ParameterLocation.Path)]
        [OpenApiParameter("ItemCategory", Type = typeof(string), In = ParameterLocation.Path)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> DeleteItem(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "item/{id}/{ItemCategory}")] HttpRequest request,
            string id,
            string ItemCategory,
            ILogger log)
        {
            var itemSvc = new ItemService(new Repositories.ItemRepository(_client));
            var succeed = await itemSvc.DeleteAsync(id, new Dictionary<string, string>() { { "ItemCategory", ItemCategory } });
            if (succeed)
            {
                return new OkObjectResult("Item Deleted");
            }
            return new OkObjectResult("Item Failed to Delete");
        }

        [FunctionName("UpdateItem")]
        public async Task<IActionResult> UpdateItem(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "item")] HttpRequest request,
            ILogger log)
        {
            string requestBody = new StreamReader(request.Body).ReadToEnd();
            var itemSvc = new ItemService(new Repositories.ItemRepository(_client));
            var res = await itemSvc.UpdateAsync(requestBody);
            return new OkObjectResult(res);
        }

        //[FunctionName("UpdateItemStock")]
        //public async Task<IActionResult> UpdateItemStock(
        //[HttpTrigger(AuthorizationLevel.Function, "put", Route = "item/stock")] HttpRequest request,
        //[CosmosDB(ConnectionStringSetting = "f39-cosmos-tutorial-string")] DocumentClient client,
        //ILogger log)
        //{
        //    string requestBody = new StreamReader(request.Body).ReadToEnd();
        //    dynamic bodyPayload = JsonConvert.DeserializeObject(requestBody);
        //    var itemRepository = new Repositories.ItemRepository(client);
        //    string ItemCategory = bodyPayload.ItemCategory;
        //    string itemId = bodyPayload.ItemId;
        //    var documentItem = await itemRepository.GetByIdAsync(itemId, partitionKeys: new Dictionary<string, string>() { { "ItemCategory", ItemCategory } });
        //    int quantity = bodyPayload.ItemQuantity;
        //    documentItem.ItemStock = documentItem.ItemStock - quantity;
        //    var result = await itemRepository.UpdateAsync(itemId, documentItem);
        //    return new OkObjectResult(documentItem);
        //}

        //[FunctionName("GetAllOrder")]
        //public async Task<IActionResult> GetAllOrder(
        //    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "order")] HttpRequest request,
        //    [CosmosDB(ConnectionStringSetting = "f39-cosmos-tutorial-string")] DocumentClient client,
        //    ILogger log)
        //{
        //    var orderRepository = new Repositories.OrdersRepository(client);
        //    var result = await orderRepository.GetAsync();
        //    return new OkObjectResult(result);
        //}

        //[FunctionName("CreateOrder")]
        //public async Task<IActionResult> CreateOrder(
        //[HttpTrigger(AuthorizationLevel.Function, "post", Route = "order")] HttpRequest request,
        //[CosmosDB(ConnectionStringSetting = "f39-cosmos-tutorial-string")] DocumentClient client,
        //ILogger log)
        //{
        //    string requestBody = new StreamReader(request.Body).ReadToEnd();
        //    dynamic bodyPayload = JsonConvert.DeserializeObject(requestBody);
        //    List<OrderItem> itemList = new List<OrderItem>();
        //    foreach (dynamic items in bodyPayload.Items) {
        //        OrderItem item = new OrderItem() 
        //        { 
        //            ItemId = items.ItemId,
        //            ItemCategory = items.ItemCategory,
        //            ItemName = items.ItemName,
        //            ItemPrice = items.ItemPrice,
        //            ItemQuantity = items.ItemQuantity
        //        };
        //        itemList.Add(item);
        //    }
        //    Orders newOrder = new Orders
        //    {
        //        OrderCategory = bodyPayload.OrderCategory,
        //        PaymentMethod = bodyPayload.PaymentMethod,
        //        TotalQuantity = bodyPayload.TotalQuantity,
        //        TotalPrice = bodyPayload.TotalPrice,
        //        Items = itemList
        //    };
        //    var ordersRepository = new Repositories.OrdersRepository(client);
        //    var result = await ordersRepository.CreateAsync(newOrder);
        //    return new OkObjectResult(result);
        //}


    }
}
