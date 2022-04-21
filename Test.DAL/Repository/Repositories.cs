using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents.Client;
using Nexus.Base.CosmosDBRepository;
using System;
using System.Collections.Generic;
using System.Text;
using Test.DAL.Models;

namespace Test.DAL.Repository
{
    public class Repositories
    {
        private static readonly string _eventGridKey = Environment.GetEnvironmentVariable("eventGridKey");
        private static readonly string _eventGridEndPoint = Environment.GetEnvironmentVariable("eventGridEndpoint");

        public class ItemRepository: DocumentDBRepository<Item>
        {
            public ItemRepository(CosmosClient client): base("tutorial", client, partitionProperties: "ItemCategory", eventGridEndPoint: _eventGridEndPoint, eventGridKey: _eventGridKey)
            {

            }
        }
        public class OrdersRepository : DocumentDBRepository<Orders>
        {
            public OrdersRepository(CosmosClient client): base("tutorial", client, partitionProperties: "OrderCategory", eventGridEndPoint: _eventGridEndPoint, eventGridKey: _eventGridKey)
            {

            }
        }
    }
}
