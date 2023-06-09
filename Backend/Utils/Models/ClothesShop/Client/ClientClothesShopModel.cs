using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Utils.Models.Database;

namespace Backend.Utils.Models.ClothesShop.Client
{
	public class ClientClothesShopModel
	{
		public List<ClothesShopItemModel> Items { get; set; }
		public int Type { get; set; }

		public ClientClothesShopModel() { }

		public ClientClothesShopModel(List<ClothesShopItemModel> items, int type)
		{
			Items = items;
			Type = type;
		}
	}
}