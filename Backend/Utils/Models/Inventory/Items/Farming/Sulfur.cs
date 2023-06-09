using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Farming
{
	public class Sulfur : ItemBase
	{
		public override int Id => 51;

		public Sulfur() : base("Schwefel", 0.5f, 15) { }

		public override Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			return Task.FromResult(false);
		}
	}
}