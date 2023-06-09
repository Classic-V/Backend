using Backend.Utils.Models.Entities;

namespace Backend.Utils.Models.Inventory.Items
{
	public class Laptop : ItemBase
	{
		public override int Id => 12;

		public Laptop() : base("Laptop", 3f, 1)
		{
		}

		public override Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			if (player.DbModel.Laptop) return Task.FromResult(false);

			player.DbModel.Inventory.RemoveItem(slot, amount);
			player.DbModel.Laptop = true;

			return Task.FromResult(true);
		}
	}
}