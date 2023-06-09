using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Drugs
{
	public class Meth : ItemBase
	{
		public override int Id => 20;

		public Meth() : base("Meth", 0.1f, 5) { }

		public override async Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			// animation
			await player.StartInteraction(() =>
			{
				player.DrugState.Meth.Active = true;
				player.DrugState.Meth.UsedTime = DateTime.Now;
				player.DbModel.Inventory.RemoveItem(slot, 1);
			}, 10000);

			return true;
		}
	}
}