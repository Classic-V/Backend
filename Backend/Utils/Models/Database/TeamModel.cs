using AltV.Net.Data;
using Backend.Utils.Enums;
using Backend.Utils.Models.Inventory;
using Backend.Utils.Models.Team;
using Backend.Utils.Models.Team.Client;
using Backend.Utils.Models.Vehicle;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Utils.Models.Database
{
	public class TeamModel
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string ShortName { get; set; } = string.Empty;
		public Position Position { get; set; } = new();
		public RgbModel Color { get; set; } = new(0, 0, 0, 0);
		public byte BlipColor { get; set; } = 0;
		public TeamType Type { get; set; }
		public int Warns { get; set; }
		public string MeeleWeapon { get; set; } = string.Empty;
		public uint MeeleWeaponHash { get; set; }
		public int Drug { get; set; }
		public int ProcessedDrug { get; set; }
		public int Money { get; set; }
		public List<TeamClientBankData> BankHistory { get; set; } = new();
		public Position Laboratory { get; set; }
		public InventoryModel LabFuel { get; set; } = new(144f, 8, InventoryType.LABORATORY_FUEL);
		public List<TeamInventoryModel> Inventories { get; set; } = new();

		[NotMapped]
		public bool HasBeenRobbed { get; set; } = false;
		[NotMapped]
		public InventoryModel LabRobbedInventory { get; set; } = new(8000f, 400, InventoryType.LABORATORY_ROB);

		public static List<TeamInventoryModel> ConvertInventories(List<TeamDatabaseInventoryModel> list)
		{
			var result = new List<TeamInventoryModel>();
			list.ForEach(x => result.Add(x.Convert()));
			return result;
		}

		public static List<TeamDatabaseInventoryModel> ConvertInventories(List<TeamInventoryModel> list)
		{
			var result = new List<TeamDatabaseInventoryModel>();
			list.ForEach(x => result.Add(x.Convert()));
			return result;
		}
	}
}