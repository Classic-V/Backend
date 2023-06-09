using Backend.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Database
{
	public class DatabaseInventoryModel
	{
		public List<DatabaseItemModel> Items { get; set; }
		public float MaxWeight { get; set; }
		public int Slots { get; set; }
		public InventoryType Type { get; set; }

		public DatabaseInventoryModel()
		{
			Items = new List<DatabaseItemModel>();
			MaxWeight = 0;
			Slots = 0;
		}

		public DatabaseInventoryModel(float maxWeight, int slots, InventoryType type)
		{
			Items = new List<DatabaseItemModel>();
			MaxWeight = maxWeight;
			Slots = slots;
			Type = type;
		}

		public DatabaseInventoryModel(List<DatabaseItemModel> items, float maxWeight, int slots, InventoryType type)
		{
			Items = items;
			MaxWeight = maxWeight;
			Slots = slots;
			Type = type;
		}
	}
}