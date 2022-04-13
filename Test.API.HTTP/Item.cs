using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.API.HTTP
{
    public class Item
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("itemName")]
        public string ItemName { get; set; }

        [JsonProperty("itemPrice")]
        public int ItemPrice { get; set; }

        [JsonProperty("itemStock")]
        public int ItemStock { get; set; }
    }
}
