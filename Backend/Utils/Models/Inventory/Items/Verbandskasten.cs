using Backend.Utils.Enums;
using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items
{
	public class Verbandskasten : ItemBase
	{
		public override int Id => 1;

		public Verbandskasten() : base("Verbandskasten", 0.5f, 10) { }

		public override async Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			player.PlayAnimation(AnimationType.USE_MEDIKIT);
			await player.StartInteraction(() =>
			{
				player.SetHealth((ushort)(player.DrugState.Heroin.Active ? 230 : 200), player.Armor);
				player.DbModel.Inventory.RemoveItem(slot, 1);
			}, player.DrugState.Ecstasy.Active ? 2000 : 4000);

			return true;
		}
	}
}