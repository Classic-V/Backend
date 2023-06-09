using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items
{
	public class Handcuffs : ItemBase
	{
		public override int Id => 10;

		public Handcuffs() : base("Handschellen", 0.5f, 3) { }

		public override Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			return Task.FromResult(false);
		}
	}
}