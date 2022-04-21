using Moq;
using Newtonsoft.Json;
using Nexus.Base.CosmosDBRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Test.DAL.Models;
using Xunit;

namespace Test.BLL.Test
{
    public class ItemServiceTest
    {
        public class GetAllItem
        {
            [Fact]
            public async Task GetAllItem_ResultFound()
            {
                IEnumerable<Item> items = new List<Item> {
                    { new Item() { ItemName = "Baju", ItemPrice = 10000, ItemCategory = "baju", ItemStock = 10 } },
                    { new Item() { ItemName = "Jaket", ItemPrice = 100000, ItemCategory = "jaket", ItemStock = 100 } }
                };

                var mockRepo = new Mock<IDocumentDBRepository<Item>>();
                mockRepo.Setup(c => c.GetAsync(
                    null,
                    It.IsAny<Func<IQueryable<Item>, IOrderedQueryable<Item>>>(),
                    It.IsAny<Expression<Func<Item, Item>>>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<TimeSpan?>(),
                    It.IsAny<string>())).Returns(Task.FromResult(new PageResult<Item>(items, "", diagnostic: "")));

                var svc = new ItemService(mockRepo.Object);

                var res = await svc.GetAllItem();

                Assert.Equal(res, items);

            }

            //[Theory]
            //[InlineData("baju")]
            //[InlineData("jaket")]
            //public async Task GetItemByCategory_ResultFound(string itemCategory)
            //{
            //    IEnumerable<Item> items = new List<Item> {
            //        { new Item() { ItemName = "Baju", ItemPrice = 10000, ItemCategory = "baju", ItemStock = 10 } },
            //        { new Item() { ItemName = "Baju Panjang", ItemPrice = 20000, ItemCategory = "baju", ItemStock = 10 } },
            //        { new Item() { ItemName = "Jaket", ItemPrice = 100000, ItemCategory = "jaket", ItemStock = 100 } }
            //    };

            //    var findItem = items.Where(o => o.ItemCategory == itemCategory);

            //    var mockRepo = new Mock<IDocumentDBRepository<Item>>();
            //    mockRepo.Setup(c => c.GetAsync(
            //        p => p.ItemCategory == itemCategory,
            //        It.IsAny<Func<IQueryable<Item>, IOrderedQueryable<Item>>>(),
            //        It.IsAny<Expression<Func<Item, Item>>>(),
            //        It.IsAny<bool>(),
            //        It.IsAny<string>(),
            //        It.IsAny<int>(),
            //        It.IsAny<Dictionary<string, string>>(),
            //        It.IsAny<bool>(),
            //        It.IsAny<bool>(),
            //        It.IsAny<TimeSpan?>(),
            //        It.IsAny<string>())).Returns(Task.FromResult(new PageResult<Item>(findItem, "", diagnostic: "")));

            //    var svc = new ItemService(mockRepo.Object);

            //    var res = await svc.GetItemByCategory(itemCategory);

            //    Assert.Equal(res, findItem);
            //    Assert.Empty()
            //    Assert.Equal(res.FirstOrDefault().ItemCategory, itemCategory);
            //}
        }

        public class CreateItem
        {
            [Theory]
            [InlineData("{ \"ItemCategory\": \"baju\", \"ItemName\": \"baju classic\", \"ItemPrice\": 10000, \"ItemStock\": 10 }")]
            [InlineData("{ \"ItemCategory\": \"jaket\", \"ItemName\": \"jaket classic\", \"ItemPrice\": 100000, \"ItemStock\": 10 }")]
            public async Task CreateItem_ItemCreated(string jsonItem)
            {
                dynamic bodyPayload = JsonConvert.DeserializeObject(jsonItem);
                Item newItem = new Item
                {
                    ItemCategory = bodyPayload.ItemCategory,
                    ItemName = bodyPayload.ItemName,
                    ItemPrice = bodyPayload.ItemPrice,
                    ItemStock = bodyPayload.ItemStock
                };
                var mockRepo = new Mock<IDocumentDBRepository<Item>>();

                List<Item> items = new List<Item>();

                mockRepo.Setup(c => c.CreateAsync(
                It.IsAny<Item>(),
                It.IsAny<EventGridOptions>(),
                It.IsAny<string>(),
                It.IsAny<string>())).ReturnsAsync((Item p, EventGridOptions evg, string str1, string str2) => p)
                .Callback((Item p, EventGridOptions evg, string createdBy, string activeFlag) =>
                {
                    items.Add(p);
                });

                var svc = new ItemService(mockRepo.Object);

                var res = await svc.CreateItem(jsonItem);

                string itemName = bodyPayload.ItemName;

                Assert.NotEmpty(items);
                Assert.Equal(items[0].ItemName, itemName);
            }
        }

        public class GetByIdAsync
        {
            [Theory]
            [InlineData("1234", "baju")]
            [InlineData("4567", "baju")]
            public async Task GetByIdAndCategory_Success(string id, string category)
            {
                var partitionKey = new Dictionary<string, string>() { { "ItemCategory", category } };

                List<Item> items = new List<Item>();

                items.Add(new Item() { Id = "1234", ItemName = "baju classic", ItemCategory = "baju", ItemPrice = 10000, ItemStock = 10});
                items.Add(new Item() { Id = "4567", ItemName = "baju premium", ItemCategory = "baju", ItemPrice = 20000, ItemStock = 20});

                var resItems = items.Where(p => p.Id == id).FirstOrDefault();

                var mockRepo = new Mock<IDocumentDBRepository<Item>>();

                mockRepo.Setup(c => c.GetByIdAsync(
                   It.IsAny<string>(),
                   It.IsAny<Dictionary<string, string>>())).Returns(Task.FromResult(resItems));

                var svc = new ItemService(mockRepo.Object);

                var res = await svc.GetByIdAsync(id, partitionKey);

                Assert.Equal(res, resItems);
            }
        }

        public class DeleteAsync
        {
            [Theory]
            [InlineData("1234", "baju")]
            [InlineData("4567", "baju")]
            public async Task DeleteByIdAndCategory_Success(string itemId, string itemCategory)
            {
                var partitionKey = new Dictionary<string, string>() { { "ItemCategory", itemCategory } };

                List<Item> items = new List<Item>();

                items.Add(new Item() { Id = "1234", ItemName = "baju classic", ItemCategory = "baju", ItemPrice = 10000, ItemStock = 10 });
                items.Add(new Item() { Id = "4567", ItemName = "baju premium", ItemCategory = "baju", ItemPrice = 20000, ItemStock = 20 });
                items.Add(new Item() { Id = "8910", ItemName = "baju leather", ItemCategory = "baju", ItemPrice = 30000, ItemStock = 10 });

                var resItems = items.Remove(items.Where(x => x.Id == itemId && x.ItemCategory == itemCategory).First());

                var mockRepo = new Mock<IDocumentDBRepository<Item>>();

                mockRepo.Setup(c => c.DeleteAsync(
                   It.IsAny<string>(),
                   It.IsAny<Dictionary<string, string>>(),
                   It.IsAny<EventGridOptions>())).Callback((string id, Dictionary<string, string> partitionKey, EventGridOptions options) =>
                {
                    items.Remove(items.Where(x => x.Id == id && x.ItemCategory == partitionKey["ItemCategory"]).First());
                });

                var svc = new ItemService(mockRepo.Object);

                var res = await svc.GetByIdAsync(itemId, partitionKey);

                Assert.Null(items.FirstOrDefault(x => x.Id == itemId && x.ItemCategory == itemCategory));
            }
        }

        public class UpdateAsync
        {
            [Theory]
            [InlineData("{ \"Id\": \"1234\", \"ItemCategory\": \"baju\", \"ItemName\": \"baju aja\", \"ItemPrice\": 10000, \"ItemStock\": 10 }")]
            [InlineData("{ \"Id\": \"4567\", \"ItemCategory\": \"baju\", \"ItemName\": \"jaket aja\", \"ItemPrice\": 100000, \"ItemStock\": 10 }")]
            public async Task UpdateByIdAndCategory_Success(string requestBody)
            {
                dynamic bodyPayload = JsonConvert.DeserializeObject(requestBody);
                string ItemCategory = bodyPayload.ItemCategory;
                string itemId = bodyPayload.Id;
                string ItemName = bodyPayload.ItemName;
                int ItemPrice = bodyPayload.ItemPrice;
                int ItemStock = bodyPayload.ItemStock;

                List<Item> items = new List<Item>();

                items.Add(new Item() { Id = "1234", ItemName = "baju classic", ItemCategory = "baju", ItemPrice = 10000, ItemStock = 10 });
                items.Add(new Item() { Id = "4567", ItemName = "baju premium", ItemCategory = "baju", ItemPrice = 20000, ItemStock = 20 });
                items.Add(new Item() { Id = "8910", ItemName = "baju leather", ItemCategory = "baju", ItemPrice = 30000, ItemStock = 10 });

                var getItem = items.Where(x => x.Id == itemId && x.ItemCategory == ItemCategory).First();

                var mockRepo = new Mock<IDocumentDBRepository<Item>>();

                mockRepo.Setup(c => c.GetByIdAsync(
                   It.IsAny<string>(),
                   It.IsAny<Dictionary<string, string>>())).Returns(Task.FromResult(getItem));

                mockRepo.Setup(c => c.UpdateAsync(
                    It.IsAny<string>(),
                    It.IsAny<Item>(),
                    It.IsAny<EventGridOptions>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>()
                ))
                .ReturnsAsync(
                    (string id, Item item, EventGridOptions options, string lastUpdatedBy, bool isOptimisticConcurrency) => item)
                .Callback((string id, Item item, EventGridOptions options, string lastUpdatedBy, bool isOptimisticConcurrenc) => {
                    getItem = item;
                });

                //string id, T item, EventGridOptions options = null, string lastUpdatedBy = null, bool isOptimisticConcurrency = false

                var svc = new ItemService(mockRepo.Object);

                var res = await svc.UpdateAsync(requestBody);

                Assert.Equal(res.ItemName, ItemName);
                Assert.Equal(res.ItemPrice, ItemPrice);
                Assert.Equal(res.ItemStock, ItemStock);
            }
        }
    }
}
