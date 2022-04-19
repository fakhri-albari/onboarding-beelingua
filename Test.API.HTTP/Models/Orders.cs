using Newtonsoft.Json;
using Nexus.Base.CosmosDBRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.API.HTTP.Models
{
    public class OrderItem
    {
        public string ItemId { get; set; }
        public string ItemCategory { get; set; }
        public string ItemName { get; set; }
        public string ItemPrice { get; set; }
        public int ItemQuantity { get; set; }
    }

    public class Orders : ModelBase
    {
        [JsonProperty("OrderCategory")]
        public string OrderCategory { get; set; }

        [JsonProperty("PaymentMethod")]
        public string PaymentMethod { get; set; }

        [JsonProperty("TotalQuantity")]
        public int TotalQuantity { get; set; }

        [JsonProperty("TotalPrice")]
        public int TotalPrice { get; set; }

        [JsonProperty("Items")]
        public List<OrderItem> Items { get; set; }
    }
}
