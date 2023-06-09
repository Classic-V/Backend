using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net.Data;
using Backend.Utils.Enums;
using Backend.Utils.Models.Shops;

namespace Backend.Utils.Models.Database
{
    public class ShopModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Position Position { get; set; }
        public PositionModel PedPosition { get; set; } = new();
        public ShopType Type { get; set; }
        public List<ShopItemModel> Items { get; set; } = new();
        public int OwnerId { get; set; }
    }
}
