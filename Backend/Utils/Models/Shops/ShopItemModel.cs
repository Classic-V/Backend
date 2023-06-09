using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Shops
{
    public class ShopItemModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int Price { get; set; }

        public ShopItemModel(int itemId, string itemName, int price)
        {
            this.ItemId = itemId;
            this.ItemName = itemName;
            this.Price = price;
        }
    }
}
