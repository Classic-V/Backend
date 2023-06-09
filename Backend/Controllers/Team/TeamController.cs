using AltV.Net;
using AltV.Net.Async;
using Backend.Controllers.Team.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models.Database;
using AltV.Net.Elements.Entities;
using Backend.Utils.Models.Entities;
using Backend.Services.Team.Interface;
using AltV.Net.Data;
using Backend.Utils.Models.JumpPoint;
using Backend.Controllers.JumpPoint.Interface;
using Backend.Utils.Models.Inventory;
using Backend.Utils.Streamer;

namespace Backend.Controllers.Team
{
	public class TeamController : ITeamController
	{
		private readonly ITeamService _teamService;
		private readonly IJumpPointController _jumpPointController;

		public TeamController(ITeamService teamService, IJumpPointController jumpPointController)
		{
			_teamService = teamService;
			_jumpPointController = jumpPointController;

			LoadTeams();
		}

		private async void LoadTeams()
		{
			while (_teamService.Teams.Count == 0)
				await Task.Delay(2000);

			for (var i = 0; i < _teamService.Teams.Count; i++) LoadTeam(_teamService.Teams[i]);
		}

		public async void LoadTeam(TeamModel team)
		{
			var shape = (ClShape)Alt.CreateColShapeSphere(team.Position, 1.5f);
			shape.Id = team.Id;
			shape.ShapeType = ColshapeType.TEAM;
			shape.Size = 1.5f;

			team.Inventories.ForEach(x =>
			{
				var index = team.Inventories.IndexOf(x);

				var inv = (ClShape)Alt.CreateColShapeSphere(x.Position, 1.5f);
				inv.Id = team.Id;
				inv.ShapeType = ColshapeType.TEAM_INVENTORY;
				inv.Size = 1.5f;
				inv.Inventory = x.Inventory;
				inv.StorageInventoryId = index;
				inv.InventoryOwner = team.Id;
				inv.InventoryOwnerType = 1;

				MarkerStreamer.AddMarker(new Utils.Models.Marker(27, x.Position - new Position(0, 0, 0.95f), new Rgba(0, 155, 255, 255), 100, 0));
			});

			if(team.Type != TeamType.POLICE)
			{
				var labExitPosition = team.Type == TeamType.MAFIA ? new Position(1088.59f, -3187.83f, -38.99f) : new Position(1066.15f, -3183.54f, -39.16f);
				var labInputPosition = team.Type == TeamType.MAFIA ? new Position(1099.62f, -3195.15f, -38.99f) : new Position(1039.2f, -3205.83f, -38.16f);
				var labOutputPosition = team.Type == TeamType.MAFIA ? new Position(1101.83f, -3193.84f, -38.99f) : new Position(1036.18f, -3203.78f, -38.18f);
				var labInteractionPosition = team.Type == TeamType.MAFIA ? new Position(1087.23f, -3194.24f, -38.99f) : new Position(1044.52f, -3194.90f, -38.16f);
				var labFuelPosition = team.Type == TeamType.MAFIA ? new Position(1103.15f, -3199.00f, -38.99f) : new Position(1065.29f, -3187.60f, -39.16f);
				var labRobPosition = team.Type == TeamType.MAFIA ? new Position(1103.63f, -3195.94f, -38.99f) : new Position(1060.50f, -3182.37f, -39.18f);

				var labInput = (ClShape)Alt.CreateColShapeSphere(labInputPosition, 0.5f);
				labInput.Id = team.Id;
				labInput.ShapeType = ColshapeType.LABORATORY_INPUT;
				labInput.Size = 0.5f;
				labInput.Dimension = team.Id;
				MarkerStreamer.AddMarker(new Utils.Models.Marker(25, (labInputPosition - new Position(0, 0, 1)), new Position(1, 1, 1), new Rgba(0, 155, 255, 255), 100, false, false, false, team.Id));

				var labOutput = (ClShape)Alt.CreateColShapeSphere(labOutputPosition, 0.5f);
				labOutput.Id = team.Id;
				labOutput.ShapeType = ColshapeType.LABORATORY_OUTPUT;
				labOutput.Size = 0.5f;
				labOutput.Dimension = team.Id;
				MarkerStreamer.AddMarker(new Utils.Models.Marker(25, (labOutputPosition - new Position(0, 0, 1)), new Position(1, 1, 1), new Rgba(0, 155, 255, 255), 100, false, false, false, team.Id));

				var labInteraction = (ClShape)Alt.CreateColShapeSphere(labInteractionPosition, 1.5f);
				labInteraction.Id = team.Id;
				labInteraction.ShapeType = ColshapeType.LABORATORY_INTERACTION;
				labInteraction.Size = 0.5f;
				labInteraction.Dimension = team.Id;
				MarkerStreamer.AddMarker(new Utils.Models.Marker(25, (labInteractionPosition - new Position(0, 0, 1)), new Position(1, 1, 1), new Rgba(0, 155, 255, 255), 100, false, false, false, team.Id));

				var labFuel = (ClShape)Alt.CreateColShapeSphere(labFuelPosition, 0.5f);
				labFuel.Id = team.Id;
				labFuel.ShapeType = ColshapeType.LABORATORY_FUEL;
				labFuel.Size = 0.5f;
				labFuel.Dimension = team.Id;
				labFuel.Inventory = team.LabFuel;
				MarkerStreamer.AddMarker(new Utils.Models.Marker(25, (labFuelPosition - new Position(0, 0, 1)), new Position(1, 1, 1), new Rgba(0, 155, 255, 255), 100, false, false, false, team.Id));

				var labRob = (ClShape)Alt.CreateColShapeSphere(labRobPosition, 0.5f);
				labRob.Id = team.Id;
				labRob.ShapeType = ColshapeType.LABORATORY_ROB;
				labRob.Size = 0.5f;
				labRob.Dimension = team.Id;
				labRob.Inventory = team.LabRobbedInventory;
				labRob.Locked = true;
				MarkerStreamer.AddMarker(new Utils.Models.Marker(25, (labRobPosition - new Position(0, 0, 1)), new Position(1, 1, 1), new Rgba(255, 0, 0, 255), 100, false, false, false, team.Id));

				await _jumpPointController.CreateJumpPoint(
					new JumpPointModel(600000 + team.Id,
						new JumpPointPosition(team.Laboratory, Rotation.Zero, 1.2f, 0),
						new JumpPointPosition(labExitPosition, Rotation.Zero, 1.2f, team.Id),
						new List<int>(), new List<int>() { team.Id }, "")
					{
						Crackable = true,
						Name = $"{(team.Type == TeamType.MAFIA ? "Kokain" : "Weed")} Labor",
						OnCrack = OnLaboratoryCrack
					}, false); ;
			}
		}

		public async void OnLaboratoryCrack(ClPlayer player, JumpPointModel point)
		{
			var enemyTeam = await _teamService.GetTeam(point.Id - 600000);
			if (enemyTeam == null) return;

			foreach(var teamPlayer in ClPlayer.All.Where(x => x.DbModel != null && x.DbModel.Team == player.DbModel.Team))
			{
				await teamPlayer.Notify("LABOR", "Ihr fangt nun an das Labor aufzubrechen.", NotificationType.INFO);
			}

			foreach (var teamPlayer in ClPlayer.All.Where(x => x.DbModel != null && x.DbModel.Team == enemyTeam.Id))
			{
				await teamPlayer.Notify("LABOR", "Jemand versucht in das Labor einzubrechen.", NotificationType.INFO);
			}
		}

		public async Task RobLaboratory(ClPlayer player)
		{
			var shape = player.CurrentShape;
			if (shape == null) return;

			var team = await _teamService.GetTeam(shape.Id);
			if (team == null) return;

			var allItems = new List<ItemModel>();
			foreach(var target in ClPlayer.All.Where(x => x.DbModel != null && x.DbModel.Team == team.Id))
			{
				allItems.AddRange(target.DbModel.LaboratoryInput.Items);
				target.DbModel.LaboratoryInput.Items.Clear();
				allItems.AddRange(target.DbModel.LaboratoryOutput.Items);
				target.DbModel.LaboratoryOutput.Items.Clear();
			}

			var items = new List<ItemModel>();
			for (var i = 0; i < allItems.Count; i++)
				items.Add(new ItemModel(allItems[i].Model, allItems[i].Amount, i + 1));

				team.LabRobbedInventory.Items = items;

			await player.StartInteraction(async () =>
			{
				var robShape = ClShape.Get(x => x.Id == team.Id && x.ShapeType == ColshapeType.LABORATORY_ROB);
				if (robShape == null) return;

				robShape.Locked = false;
				team.HasBeenRobbed = true;
				await player.Notify("INFORMATION", "Du hast das Labor gehackt!", NotificationType.SUCCESS);
			}, 3000/*00*/);
		}
	}
}