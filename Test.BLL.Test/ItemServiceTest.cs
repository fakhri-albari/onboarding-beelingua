using Moq;
using Nexus.Base.CosmosDBRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Test.DAL.Models;
using Xunit;

namespace Test.BLL.Test
{
    public class ItemServiceTest
    {
        public class GetAllItem
        {
            public async Task GetAllItem_ResultFound()
            {
                var mockRepo = new Mock<IDocumentDBRepository<Item>>();
                mockRepo.Setup(c => c.GetAsync(predicate: p => true));
            }
        }
    }
}
