using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.API.HTTP
{
    public class Item : Resource
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("itemName")]
        public string ItemName { get; set; }

        [JsonProperty("itemPrice")]
        public int ItemPrice { get; set; }

        [JsonProperty("itemStock")]
        public int ItemStock { get; set; }

        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }
    }
}
