using Backend.Utils.Models.Entities;

namespace Backend.Utils.Models.Inventory.Items.Farming
{
	public class Kevlar : ItemBase
	{
		public override int Id => 53;

		public Kevlar() : base("Kevlar", 2f, 10) { }

		public override Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			return Task.FromResult(false);
		}
	}
}