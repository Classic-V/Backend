using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items
{
	public class Weed : ItemBase
	{
		public override int Id => 302;

		public Weed() : base("Weed", 0.5f, 40)
		{
		}

		public override Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			return Task.FromResult(true);
		}
	}
}