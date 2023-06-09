using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Farming
{
	public class SulfurOre : ItemBase
	{
		public override int Id => 41;

		public SulfurOre() : base("Schwefelerz", 0.05f, 100) { }

		public override Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			return Task.FromResult(false);
		}
	}
}