using AltV.Net;
using Backend.Utils.Enums;
using Backend.Utils.Models.Entities;

namespace Backend.Utils.Models.Inventory.Items.FIB
{
	public class FIBArmor : ItemBase
	{
		public override int Id => 34;

		public FIBArmor() : base("FIB Schutzweste", 1.5f, 10) { }

		public override async Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			if (player.DbModel.Team != 2 || !player.DbModel.Duty) return false;

			player.PlayAnimation(AnimationType.USE_VEST);
			await player.StartInteraction(() =>
			{
				var hash = Alt.Hash("mp_m_frakclothesm");
				player.SetHealth(player.Health, 100);
				player.DbModel.Clothes.Armor.Drawable = 2;
				player.DbModel.Clothes.Armor.Texture = 8;
				player.DbModel.Clothes.Armor.Dlc = hash;
				player.SetDlcClothes(9, 2, 8, 0, hash);
				player.DbModel.Inventory.RemoveItem(slot, 1);
			}, 4000);

			return true;
		}
	}
}