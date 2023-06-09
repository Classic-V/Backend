using AltV.Net;
using Backend.Modules.Inventory;
using Backend.Utils.Models.Entities;

namespace Backend.Utils.Models.Inventory.Items.Police
{
	public class PoliceClothes : ItemBase
	{
		public override int Id => 36;

		public PoliceClothes() : base("Police Einsatzkleidung", 2f, 1)
		{
		}

		public override async Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			if (player.DbModel.PlayerCachedClothes != null || player.DbModel.Team != 1 || !player.DbModel.Duty) return false;

			player.PlayAnimation(Enums.AnimationType.USE_VEST);
			await player.StartInteraction(() =>
			{
				var dlc = Alt.Hash("mp_m_fib");

				player.DbModel.PlayerCachedClothes = player.DbModel.Clothes;
				player.DbModel.Clothes = new(
					new(1, 52, 0, 0),
					new(11, 1, 1, dlc),
					new(8, 15, 0, 0),
					new(3, 17, 0, 0),
					new(4, 31, 0, 0),
					new(6, 24, 0, 0),
					player.DbModel.Clothes.Armor,
					new(5, 0, 0, 0),
					new(7, 0, 0, dlc),

					new(0, 115, 0, 0),
					new(1, -1, -1, 0),
					new(2, -1, 0, 0),
					new(6, -1, 0, 0),
					new(7, -1, 0, 0));

				player.ApplyClothes();
				player.DbModel.Inventory.RemoveItem(slot, 1);
				player.DbModel.Inventory.AddItem(InventoryModule.GetItemBase(31)!, 1);
			}, 5000);

			return true;
		}
	}
}