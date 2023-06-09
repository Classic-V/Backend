using Backend.Utils.Enums;
using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items
{
	public class GangwarWeste : ItemBase
	{
		public override int Id => 1500;

		public GangwarWeste() : base("Gangwar Schutzweste", 0f, 100) { }

		public override async Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			if (!player.IsInGangwar) return false;

			player.PlayAnimation(AnimationType.USE_VEST);
			await player.StartInteraction(() =>
			{
				player.SetHealth(player.Health, 100);
				player.DbModel.Clothes.Armor.Drawable = 15;
				player.DbModel.Clothes.Armor.Texture = 2;
				player.DbModel.Clothes.Armor.Dlc = 0;
				player.SetClothes(9, 15, 2, 0);
				player.DbModel.Inventory.RemoveItem(slot, 1);
			}, 4000);

			return true;
		}
	}
}