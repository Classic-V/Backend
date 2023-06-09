using Backend.Utils.Models.Entities;

namespace Backend.Utils.Models.Inventory.Items
{
	public class ClothesBag : ItemBase
	{
		public override int Id => 31;

		public ClothesBag() : base("Kleidersack", 2f, 1)
		{
		}

		public override async Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			if (player.DbModel.PlayerCachedClothes == null) return false;

			player.PlayAnimation(Enums.AnimationType.USE_VEST);
			await player.StartInteraction(() =>
			{
				if (player.DbModel.PlayerCachedClothes == null) return;

				player.DbModel.Clothes = player.DbModel.PlayerCachedClothes;
				player.DbModel.PlayerCachedClothes = null;
				player.ApplyClothes();
				player.DbModel.Inventory.RemoveItem(slot, 1);
			}, 5000);

			return true;
		}
	}
}