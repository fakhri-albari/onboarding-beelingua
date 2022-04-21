using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Nexus.Base.CosmosDBRepository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Test.DAL.Models;
using Test.DAL.Repository;

namespace Test.BLL
{
    public class ItemService
    {
        private readonly IDocumentDBRepository<Item> _repo;

        public ItemService(IDocumentDBRepository<Item> repo)
        {
            _repo = repo;
        }

        public async Task<Item> CreateItem(string requestBody)
        {
            Console.WriteLine(requestBody);
            dynamic bodyPayload = JsonConvert.DeserializeObject(requestBody);
            Item newItem = new Item
            {
                ItemCategory = bodyPayload.ItemCategory,
                ItemName = bodyPayload.ItemName,
                ItemPrice = bodyPayload.ItemPrice,
                ItemStock = bodyPayload.ItemStock
            };
            var result = await _repo.CreateAsync(newItem);
            return newItem;
        }

        public async Task<IEnumerable<Item>> GetAllItem()
        {
            var result = await _repo.GetAsync();
            //var result = await _repo.GetAsync(predicate: p => true);
            return result.Items;
        }

        public async Task<Item> GetByIdAsync(string id, Dictionary<string, string> partitionKey)
        {
            var result = await _repo.GetByIdAsync(id, partitionKeys: partitionKey);
            return result;
        }

        public async Task<bool> DeleteAsync(string id, Dictionary<string, string> partitionKey)
        {
            try
            {
                await _repo.DeleteAsync(id, partitionKeys: partitionKey);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<Item> UpdateAsync(string requestBody)
        {
            dynamic bodyPayload = JsonConvert.DeserializeObject(requestBody);
            string ItemCategory = bodyPayload.ItemCategory;
            string itemId = bodyPayload.ItemId;
            var documentItem = await _repo.GetByIdAsync(itemId, partitionKeys: new Dictionary<string, string>() { { "ItemCategory", ItemCategory } });
            documentItem.ItemName = bodyPayload.ItemName;
            documentItem.ItemPrice = bodyPayload.ItemPrice;
            documentItem.ItemStock = bodyPayload.ItemStock;
            var result = await _repo.UpdateAsync(itemId, documentItem);
            return result;
        }

        //public async Task<IEnumerable<Item>> GetItemByCategory(string category)
        //{
        //    var result = await _repo.GetAsync(p => p.ItemCategory == category);
        //    //var result = await _repo.GetAsync(predicate: p => true);
        //    return result.Items;
        //}
    }
}
