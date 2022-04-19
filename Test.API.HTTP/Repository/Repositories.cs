using Microsoft.Azure.Documents.Client;
using Nexus.Base.CosmosDBRepository;
using System;
using System.Collections.Generic;
using System.Text;
using Test.API.HTTP.Models;

namespace Test.API.HTTP.Repository
{
    public class Repositories
    {
        private static readonly string _eventGridKey = Environment.GetEnvironmentVariable("eventGridKey");
        private static readonly string _eventGridEndPoint = Environment.GetEnvironmentVariable("eventGridEndpoint");

        public class ItemRepository: DocumentDBRepository<Item>
        {
            public ItemRepository(DocumentClient client): base("tutorial", client, partitionProperties: "ItemCategory", eventGridEndPoint: _eventGridEndPoint, eventGridKey: _eventGridKey)
            {

            }
        }
        public class OrdersRepository : DocumentDBRepository<Orders>
        {
            public OrdersRepository(DocumentClient client): base("tutorial", client, partitionProperties: "OrderCategory", eventGridEndPoint: _eventGridEndPoint, eventGridKey: _eventGridKey)
            {

            }
        }
    }
}
