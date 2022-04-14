using Microsoft.Azure.Documents.Client;
using Nexus.Base.CosmosDBRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.API.HTTP.Repository
{
    public class Repositories
    {
        public class ItemRepository: DocumentDBRepository<Item>
        {
            public ItemRepository(DocumentClient client): base("tutorial", client, partitionProperties: "ItemCategory")
            {

            }
        }
    }
}
