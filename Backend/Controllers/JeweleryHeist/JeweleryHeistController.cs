using AltV.Net;
using Backend.Controllers.JeweleryHeist.Interface;
using Backend.Modules.Scenario;
using Backend.Services.Team.Interface;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Inventory.Items;
using Backend.Utils.Models.Inventory;
using Backend.Modules.Inventory;
using Backend.Services.Team;
using Backend.Utils.Enums;
using Newtonsoft.Json;

namespace Backend.Controllers.JeweleryHeist
{
	public class JeweleryHeistController : IJeweleryHeistController
	{
		public bool IsBeingRobbed { get; private set; }
		public bool HasBeenAttacked { get; private set; }

		private readonly List<ClShape> _lootShapes;
		private readonly List<ItemModel> Loot = new();

		private readonly ITeamService _teamService;

		public JeweleryHeistController(ITeamService teamService)
		{
			HasBeenAttacked = false;

			_lootShapes = new();

			_teamService = teamService;

			var shape = (ClShape)Alt.CreateColShapeSphere(JeweleryHeistModule.Position, 1f);
			shape.Id = 0;
			shape.ShapeType = ColshapeType.JEWELERY_MAIN;
			shape.Size = 1f;
		}

		public async Task<bool> StartHeist(ClPlayer player)
		{
			if (HasBeenAttacked)
			{
				await player.Notify("JUWELIER", $"Der Juwelier wurde bereits ausgeraubt!", NotificationType.ERROR);
				return false;
			}

			Loot.Clear();
			var loot = InventoryModule.GetItemBase(10001)!;
			for (var i = 0; i < 16; i++)
			{
				Loot.Add(new ItemModel(loot, 20, i + 1));
			}

			HasBeenAttacked = true;
			await NotifyTeam(player.DbModel.Team, "Deine Fraktion fängt nun an den Juwelier zu hacken!");
			await NotifyFederalTeams("Es wurde ein Alarm im Juwelier gemeldet!");
			return true;
		}

		public async Task StopHeist()
		{
			IsBeingRobbed = false;
			await DestroyLootShapes();
		}

		public async Task UnlockLoot(ClPlayer player)
		{
			if (player.Position.Distance(JeweleryHeistModule.Position) > 5) return;

			IsBeingRobbed = true;
			await NotifyFederalTeams("Die Sicherheitssysteme sind gerade ausgefallen!");
			await CreateLootShapes();
		}

		private async Task CreateLootShapes()
		{
			for (var i = 0; i < JeweleryHeistModule.LootPositions.Count; i++)
			{
				var shape = (ClShape)Alt.CreateColShapeSphere(JeweleryHeistModule.LootPositions[i], 1f);
				shape.Id = i + 1;
				shape.ShapeType = ColshapeType.JEWELERY_LOOT;
				shape.Size = 1f;
				shape.Inventory = await GetLootInventory();

				_lootShapes.Add(shape);
			}
		}

		private Task<InventoryModel> GetLootInventory()
		{
			return Task.FromResult(new InventoryModel(new List<ItemModel>(Loot), 64f, 16, InventoryType.JEWELERY_LOOT));
		}

		private Task DestroyLootShapes()
		{
			for (var i = 0; i < _lootShapes.Count; i++) _lootShapes[i].Destory();
			_lootShapes.Clear();

			return Task.CompletedTask;
		}

		private async Task NotifyTeam(int team, string message)
		{
			for (var i = 0; i < ClPlayer.All.Count; i++)
			{
				if (ClPlayer.All[i].DbModel == null || ClPlayer.All[i].DbModel.Team != team) return;

				await ClPlayer.All[i].Notify("JUWELIER", message, NotificationType.INFO);
			}
		}

		private async Task NotifyFederalTeams(string message)
		{
			for (var i = 0; i < ClPlayer.All.Count; i++)
			{
				if (ClPlayer.All[i].DbModel == null || ClPlayer.All[i].DbModel.Team < 1) return;

				var team = await _teamService.GetTeam(ClPlayer.All[i].DbModel.Team);
				if (team == null || team.Type != TeamType.POLICE) return;

				await ClPlayer.All[i].Notify("JUWELIER", message, NotificationType.INFO);
			}
		}
	}
}