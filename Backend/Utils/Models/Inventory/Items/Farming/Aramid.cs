using Backend.Utils.Models.Entities;

namespace Backend.Utils.Models.Inventory.Items.Farming
{
	public class Aramid : ItemBase
	{
		public override int Id => 52;

		public Aramid() : base("Aramidfasern", 0.05f, 80) { }

		public override Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			return Task.FromResult(false);
		}
	}
}