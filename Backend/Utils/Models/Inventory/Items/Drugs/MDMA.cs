using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Drugs
{
	public class MDMA : ItemBase
	{
		public override int Id => 21;

		public MDMA() : base("MDMA", 0.1f, 5) {}

		public override async Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			// Todo: play animation
			await player.StartInteraction(() =>
			{
				player.DrugState.MDMA.Active = true;
				player.DrugState.MDMA.UsedTime = DateTime.Now;
				player.Emit("Client:PlayerModule:SetRunSpeedMultiplier", 1.25);
				player.DbModel.Inventory.RemoveItem(slot, 1);
			}, 10000);

			return true;
		}
	}
}