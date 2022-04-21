using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Nexus.Base.CosmosDBRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.DAL.Models
{
    public class Item : ModelBase
    {
        [JsonProperty("ItemCategory")]
        public string ItemCategory { get; set; }

        [JsonProperty("ItemName")]
        public string ItemName { get; set; }

        [JsonProperty("ItemPrice")]
        public int ItemPrice { get; set; }

        [JsonProperty("ItemStock")]
        public int ItemStock { get; set; }
    }
}
