using AltV.Net.Data;
using Backend.Modules.Inventory;
using Backend.Utils.Models.Inventory.Database;

namespace Backend.Utils.Models.Team
{
	public class TeamDatabaseInventoryModel
	{
		public DatabaseInventoryModel Inventory { get; set; }
		public Position Position { get; set; }

		public TeamDatabaseInventoryModel(DatabaseInventoryModel inventory, Position position)
		{
			Inventory = inventory;
			Position = position;
		}

		public TeamInventoryModel Convert()
		{
			return new TeamInventoryModel(InventoryModule.Convert(Inventory), Position);
		}
	}
}