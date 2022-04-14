using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Nexus.Base.CosmosDBRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.API.HTTP
{
    public class Item : ModelBase
    {

        [JsonProperty("itemName")]
        public string ItemName { get; set; }

        [JsonProperty("itemPrice")]
        public int ItemPrice { get; set; }

        [JsonProperty("itemStock")]
        public int ItemStock { get; set; }
    }
}
