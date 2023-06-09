using AltV.Net;
using Backend.Utils.Enums;
using Backend.Utils.Models.Entities;

namespace Backend.Utils.Models.Inventory.Items.Police
{
	public class PoliceArmor : ItemBase
	{
		public override int Id => 33;

		public PoliceArmor() : base("Police Schutzweste", 1.5f, 10) { }

		public override async Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			if (player.DbModel.Team != 1 || !player.DbModel.Duty) return false;

			player.PlayAnimation(AnimationType.USE_VEST);
			await player.StartInteraction(() =>
			{
				var hash = Alt.Hash("mp_m_frakclothesm");
				player.SetHealth(player.Health, 100);
				player.DbModel.Clothes.Armor.Drawable = 2;
				player.DbModel.Clothes.Armor.Texture = 0;
				player.DbModel.Clothes.Armor.Dlc = hash;
				player.SetDlcClothes(9, 2, 0, 0, hash);
				player.DbModel.Inventory.RemoveItem(slot, 1);
			}, 4000);

			return true;
		}
	}
}