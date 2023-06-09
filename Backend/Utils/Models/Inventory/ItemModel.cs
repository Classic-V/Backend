using Backend.Utils.Models.Entities;

namespace Backend.Utils.Models.Inventory
{
	public class ItemModel
	{
		public ItemBase Model { get; }
		public int Amount { get; set; }
		public int Slot { get; set; }

		public ItemModel(ItemBase model, int amount, int slot)
		{
			Model = model;
			Amount = amount;
			Slot = slot;
		}

		public async Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			return await Model.Use(player, slot, amount);
		}
	}
}