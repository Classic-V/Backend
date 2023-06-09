using Backend.Utils.Models.Entities;

namespace Backend.Utils.Models.Inventory.Items
{
	public class CokeBox : ItemBase
	{
		public override int Id => 401;

		public CokeBox() : base("Kokain Kiste", 20f, 1)
		{
		}

		public override Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			return Task.FromResult(true);
		}
	}
}