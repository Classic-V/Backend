using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items
{
	public class Battery : ItemBase
	{
		public override int Id => 501;

		public Battery() : base("Batterie", 0.3f, 60)
		{
		}

		public override Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			return Task.FromResult(true);
		}
	}
}