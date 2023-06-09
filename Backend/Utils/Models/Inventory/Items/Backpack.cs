using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items
{
	public class Backpack : ItemBase
	{
		public override int Id => 5;

		public Backpack() : base("Rucksack", 5f, 1)
		{
		}

		public override Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			if (player.DbModel.Backpack) return Task.FromResult(false);

			player.DbModel.Backpack = true;
			player.DbModel.Inventory.Slots = 16;
			player.DbModel.Inventory.MaxWeight = 70f;
			player.DbModel.Inventory.RemoveItem(slot, 1);

			return Task.FromResult(true);
		}
	}
}