using Backend.Utils.Models.Entities;

namespace Backend.Utils.Models.Inventory.Items
{
	public class Box : ItemBase
	{
		public override int Id => 8;

		public Box() : base("Karton", 0.5f, 2)
		{
		}

		public override Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			return Task.FromResult(true);
		}
	}
}