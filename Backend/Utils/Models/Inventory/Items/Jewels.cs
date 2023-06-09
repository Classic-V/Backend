using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items
{
	public class Jewels : ItemBase
	{
		public override int Id => 10001;

		public Jewels() : base("Juwelen", 0.2f, 20)
		{
		}

		public override Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			return Task.FromResult(true);
		}
	}
}