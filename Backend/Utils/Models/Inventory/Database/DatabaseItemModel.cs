using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Database
{
	public class DatabaseItemModel
	{
		public int ItemId { get; set; }
		public int Amount { get; set; }
		public int Slot { get; set; }

		public DatabaseItemModel()
		{
			ItemId = 0;
			Amount = 0;
			Slot = 0;
		}

		public DatabaseItemModel(int itemId, int amount, int slot)
		{
			ItemId = itemId;
			Amount = amount;
			Slot = slot;
		}
	}
}