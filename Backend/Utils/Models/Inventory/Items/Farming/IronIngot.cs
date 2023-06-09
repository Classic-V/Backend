using Backend.Utils.Models.Entities;

namespace Backend.Utils.Models.Inventory.Items.Farming
{
	public class IronIngot : ItemBase
	{
		public override int Id => 50;

		public IronIngot() : base("Eisenbarren", 1f, 5) { }

		public override Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			return Task.FromResult(false);
		}
	}
}