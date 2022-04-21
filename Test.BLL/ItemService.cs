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
            dynamic bodyPayload = JsonConvert.DeserializeObject(requestBody);
            Item newItem = new Item
            {
                ItemCategory = bodyPayload.ItemCategory,
                ItemName = bodyPayload.ItemName,
                ItemPrice = bodyPayload.ItemPrice,
                ItemStock = bodyPayload.ItemStock
            };
            var result = await _repo.CreateAsync(newItem);
            return result;
        }

        public async Task<dynamic> GetAllItem()
        {
            var result = await _repo.GetAsync();
            //var result = await _repo.GetAsync(predicate: p => true);
            return result.Items;
        }
    }
}
