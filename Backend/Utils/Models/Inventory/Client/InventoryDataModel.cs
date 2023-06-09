namespace Backend.Utils.Models.Inventory.Client
{
	public class InventoryDataModel
	{
		public InventoryModel Inventory { get; set; }
		public int ContainerId { get; set; }
		public InventoryModel? Container { get; set; }
		public int GiveItemTarget { get; set; }
		public List<InventoryWeaponDataModel>? Weapons { get; set; }

		public InventoryDataModel(InventoryModel inventory, int containerId, InventoryModel? container, int giveItemTarget, List<InventoryWeaponDataModel> weapons = null)
		{
			Inventory = inventory;
			ContainerId = containerId;
			Container = container;
			GiveItemTarget = giveItemTarget;
			Weapons = weapons;
		}
	}
}