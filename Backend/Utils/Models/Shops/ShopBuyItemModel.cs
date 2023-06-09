using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Shops
{
    public class ShopBuyItemModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }

        public ShopBuyItemModel() {}

        public ShopBuyItemModel(int id, string name, int amount)
        {
            Id = id;
            Name = name;
            Amount = amount;
        }
    }
}
