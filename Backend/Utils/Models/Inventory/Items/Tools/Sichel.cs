using Backend.Utils.Models.Entities;

namespace Backend.Utils.Models.Inventory.Items.Tools
{
	public class Sichel : ItemBase
	{
		public override int Id => 54;

		public Sichel() : base("Sichel", 1f, 1) { }

		public override Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			return Task.FromResult(false);
		}
	}
}