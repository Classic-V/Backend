using Backend.Utils.Enums;
using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items
{
	public class GangwarMedikit : ItemBase
	{
		public override int Id => 1501;

		public GangwarMedikit() : base("Gangwar Verbandskasten", 0f, 100) { }

		public override async Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			if (!player.IsInGangwar) return false;

			player.PlayAnimation(AnimationType.USE_MEDIKIT);
			await player.StartInteraction(() =>
			{
				player.SetHealth(200, player.Armor);
				player.DbModel.Inventory.RemoveItem(slot, 1);
			}, 4000);

			return true;
		}
	}
}