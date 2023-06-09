namespace Backend.Utils.Models.Inventory.Client
{
	public class InventoryWeaponDataModel
	{
		public int HashName { get; set; }
		public int Ammo { get; set; }

		public InventoryWeaponDataModel(uint hashName, int ammo)
		{
			HashName = (int)hashName;
			Ammo = ammo;
		}
	}
}