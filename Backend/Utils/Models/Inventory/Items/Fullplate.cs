using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Utils.Enums;

namespace Backend.Utils.Models.Inventory.Items
{
	public class Fullplate : ItemBase
	{
		public override int Id => 3;

		public Fullplate() : base("Schutzweste Fullplate", 2.5f, 10) { }

		public override async Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			player.PlayAnimation(AnimationType.USE_VEST);
			await player.StartInteraction(() =>
			{
				player.SetHealth(player.Health, 150);
				player.DbModel.Clothes.Armor.Drawable = 16;
				player.DbModel.Clothes.Armor.Texture = 2;
				player.DbModel.Clothes.Armor.Dlc = 0;
				player.SetClothes(9, 16, 2, 0);
				player.DbModel.Inventory.RemoveItem(slot, 1);
			}, player.DrugState.Ecstasy.Active ? 2000 : 4000);

			return true;
		}
	}
}