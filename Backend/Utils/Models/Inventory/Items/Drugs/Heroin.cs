using AltV.Net.Enums;
using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Drugs
{
	public class Heroin : ItemBase
	{
		public override int Id => 22;

		public Heroin() : base("Heroin", 0.1f, 5) { }

		public override async Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			// animation
			await player.StartInteraction(() =>
			{
				player.DrugState.Heroin.Active = true;
				player.DrugState.Heroin.UsedTime = DateTime.Now;
				player.DbModel.Inventory.RemoveItem(slot, 1);
				player.MaxHealth = 225;
			}, 10000);

			return true;
		}
	}
}