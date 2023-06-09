using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Shops
{
	public class ShopDataModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public List<ShopItemModel> Items { get; set; }

		public ShopDataModel() { }

		public ShopDataModel(int id, string name, List<ShopItemModel> items)
		{
			Id = id;
			Name = name;
			Items = items;
		}
	}
}