using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items
{
	public class Gold : ItemBase
	{
		public override int Id => 10000;

		public Gold() : base("Goldbarren", 3.5f, 8)
		{
		}

		public override Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			return Task.FromResult(true);
		}
	}
}