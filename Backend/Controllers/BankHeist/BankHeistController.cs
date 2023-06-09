using Backend.Controllers.BankRobbery.Interface;
using Backend.Services.Team.Interface;
using Backend.Utils.Models.Entities;
using Backend.Utils.Enums;
using Backend.Modules.Scenario;
using AltV.Net;
using Backend.Utils.Models.Inventory;
using Backend.Modules.Inventory;

namespace Backend.Controllers.BankRobbery
{
	public class BankHeistController : IBankHeistController
	{
		public bool VaultLocked { get; private set; }
		public bool HasBeenAttacked { get; set; }

		private readonly List<ClShape> _lootShapes;
		private readonly List<ItemModel> Loot = new();

		private readonly ITeamService _teamService;

		public BankHeistController(ITeamService teamService)
		{
			VaultLocked = true;
			HasBeenAttacked = false;

			_lootShapes = new();

			_teamService = teamService;

			var shape = (ClShape)Alt.CreateColShapeSphere(BankHeistModule.VaultPosition, 1f);
			shape.Id = 0;
			shape.ShapeType = ColshapeType.BANK_ROBBERY_MAIN;
			shape.Size = 1f;
		}

		public async Task<bool> StartRobbery(ClPlayer player)
		{
			if (HasBeenAttacked)
			{
				await player.Notify("STAATSBANK", $"Die Staatsbank wurde bereits ausgeraubt!", NotificationType.ERROR);
				return false;
			}

			HasBeenAttacked = true;
			await NotifyTeam(player.DbModel.Team, "Deine Fraktion fängt nun an die Staatsbank aus zu rauben!");
			await NotifyFederalTeams("Es wurde ein Alarm in der Staatsbank gemeldet!");
			return true;
		}

		public async Task UnlockVault(ClPlayer player, bool ignoreDist = false)
		{
			if (!ignoreDist && player.Position.Distance(BankHeistModule.VaultPosition) > 10) return;

			var loot = InventoryModule.GetItemBase(10000)!;
			for (var i = 0; i < 8; i++)
			{
				Loot.Add(new ItemModel(loot, 8, i + 1));
			}

			VaultLocked = false;
			await NotifyFederalTeams("Es wurde ein Alarm im Tresor der Staatsbank gemeldet!");
			await CreateLootShapes();
		}

		private async Task CreateLootShapes()
		{
			for(var i = 0; i < BankHeistModule.VaultPositions.Count; i++)
			{
				var shape = (ClShape)Alt.CreateColShapeSphere(BankHeistModule.VaultPositions[i], 1f);
				shape.Id = i+1;
				shape.ShapeType = ColshapeType.BANK_ROBBERY_LOOT;
				shape.Size = 1f;
				shape.Inventory = await GetLootInventory();

				_lootShapes.Add(shape);
			}
		}

		private Task<InventoryModel> GetLootInventory()
		{
			return Task.FromResult(new InventoryModel(new List<ItemModel>(Loot), 224f, 8, InventoryType.BANK_ROBBERY_LOOT));
		}

		private async Task NotifyTeam(int team, string message)
		{
			for(var i = 0; i < ClPlayer.All.Count; i++)
			{
				if (ClPlayer.All[i].DbModel == null || ClPlayer.All[i].DbModel.Team != team) return;

				await ClPlayer.All[i].Notify("STAATSBANK", message, NotificationType.INFO);
			}
		}

		private async Task NotifyFederalTeams(string message)
		{
			for (var i = 0; i < ClPlayer.All.Count; i++)
			{
				if (ClPlayer.All[i].DbModel == null || ClPlayer.All[i].DbModel.Team < 1) return;

				var team = await _teamService.GetTeam(ClPlayer.All[i].DbModel.Team);
				if(team == null || team.Type != TeamType.POLICE) return;

				await ClPlayer.All[i].Notify("STAATSBANK", message, NotificationType.INFO, 15000);
			}
		}
	}
}