using AltV.Net.Data;
using Backend.Modules.Inventory;
using Backend.Utils.Models.Inventory;

namespace Backend.Utils.Models.Team
{
	public class TeamInventoryModel
	{
		public InventoryModel Inventory { get; set; }
		public Position Position { get; set; }

		public TeamInventoryModel(InventoryModel inventory, Position position)
		{
			Inventory = inventory;
			Position = position;
		}

		public TeamDatabaseInventoryModel Convert()
		{
			return new TeamDatabaseInventoryModel(InventoryModule.Convert(Inventory), Position);
		}
	}
}