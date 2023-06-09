using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Database
{
    public class BarberItemModel
    {
        public int Id { get; set; }
        public int ShopId { get; set; }
        public string Name { get; set; }
        public int DrawableId { get; set; }
        public int Price { get; set; }

        public BarberItemModel() { }
        public BarberItemModel(int shopId, string name, int drawableId, int price)
        {
            ShopId = shopId;
            Name = name;
            DrawableId = drawableId;
            Price = price;
        }
    }
}
