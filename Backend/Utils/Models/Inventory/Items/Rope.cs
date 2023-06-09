using Backend.Utils.Models.Entities;

namespace Backend.Utils.Models.Inventory.Items
{
	public class Rope : ItemBase
	{
		public override int Id => 9;

		public Rope() : base("Seile", 0.5f, 3) { }

		public override Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			return Task.FromResult(false);
		}
	}
}