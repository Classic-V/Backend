using Backend.Utils.Models.Entities;

namespace Backend.Utils.Models.Inventory.Items
{
	public class Phone : ItemBase
	{
		public override int Id => 4;

		public Phone() : base("Smartphone", 0.5f, 1)
		{
		}
		
		public override Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			if (player.DbModel.Phone) return Task.FromResult(false);

			player.DbModel.Phone = true;
			player.DbModel.Inventory.RemoveItem(slot, 1);

			return Task.FromResult(true);
		}
	}
}