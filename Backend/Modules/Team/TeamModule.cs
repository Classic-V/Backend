using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Backend.Controllers.Event.Interface;
using Backend.Controllers.Gangwar.Interface;
using Backend.Controllers.Team.Interface;
using Backend.Data;
using Backend.Modules.Inventory;
using Backend.Modules.Police;
using Backend.Services.Account.Interface;
using Backend.Services.Gangwar.Interface;
using Backend.Services.Team.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Inventory;
using Backend.Utils.Models.Player.Client;
using Backend.Utils.Models.Team.Client;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;

namespace Backend.Modules.Team
{
	public class TeamModule : Module<TeamModule>, IEventColshape, IEventIntervalTenMinute
	{
		private readonly ITeamController _teamController;
		private readonly IGangwarController _gangwarController;

		private readonly ITeamService _teamService;
		private readonly IGangwarService _gangwarService;
		private readonly IAccountService _accountService;

		public TeamModule(IEventController eventController, ITeamController teamController, IGangwarController gangwarController, ITeamService teamService, IGangwarService gangwarService, IAccountService accountService) : base("Team")
		{
			_teamController = teamController;
			_gangwarController = gangwarController;

			_teamService = teamService;
			_gangwarService = gangwarService;
			_accountService = accountService;

			eventController.OnClient<int>("Server:Team:AcceptInvite", AcceptInvite);
			eventController.OnClient("Server:Team:ToggleDuty", ToggleDuty);
			eventController.OnClient("Server:Team:BuyEquip", BuyEquip);
			eventController.OnClient<int, int, bool, bool, bool>("Server:Team:UpdateMember", UpdateMember);
			eventController.OnClient<int, bool>("Server:Team:KickMember", KickMember);
			eventController.OnClient("Server:Team:Interact", OpenTeamMenu);
			eventController.OnClient<int>("Server:Team:TakeMoney", TakeMoney);
			eventController.OnClient<int>("Server:Team:GiveMoney", GiveMoney);
			eventController.OnClient("Server:Team:ToggleLaboratory", ToggleLaboratory);
		}

		private void AcceptInvite(ClPlayer player, string eventKey, int team)
		{
			if (player.DbModel == null || team <= 1) return;

			player.DbModel.Team = team;
			player.DbModel.TeamRank = 0;
			player.DbModel.TeamAdmin = false;
			player.DbModel.TeamBankPermission = false;
			player.DbModel.TeamStoragePermission = false;
			player.DbModel.TeamJoinDate = DateTime.Now;
			
			player.SetTeam(team);
			
			foreach(var target in ClPlayer.All)
			{
				if(target.DbModel == null || target.DbModel.Team != team) continue;

				target.Notify("FRAKTION", $"{player.Name} ist der Fraktion beigetreten!", NotificationType.INFO);
			}
		}

		private async void ToggleDuty(ClPlayer player, string eventKey)
		{
			if (player.DbModel == null || player.DbModel.Team < 1 || player.DbModel.Team > 2) return;

			player.DbModel.Duty = !player.DbModel.Duty;

			if(!player.DbModel.Duty)
			{
				player.DbModel.Loadout.Clear();
				await player.SetHealth(player.Health, 0);
				await player.ApplyWeapons();
			}

			await player.Notify("FRAKTION", $"Du hast den Dienst {(player.DbModel.Duty ? "betreten" : "verlassen")}.", NotificationType.INFO);
			foreach (var teamMember in ClPlayer.All.Where(x => x.DbModel != null && x.DbModel.Team == player.DbModel.Team))
				await teamMember.Notify("FRAKTION", $"{player.Name} hat den Dienst {(player.DbModel.Duty ? "betreten" : "verlassen")}.", NotificationType.INFO);
		}

		private async void ToggleLaboratory(ClPlayer player, string eventKey)
		{
			if (player.DbModel == null || player.Dimension == 0 || player.DbModel.Team != player.CurrentShape?.Id) return;

			player.LaboratoryStatus = !player.LaboratoryStatus;
			if (player.LaboratoryStatus) await player.Notify("LABOR", $"Du hast das Labor Eingeschaltet.", NotificationType.SUCCESS);
			else await player.Notify("LABOR", $"Du hast das Labor Ausgeschaltet.", NotificationType.ERROR);
		}

		private async void GiveMoney(ClPlayer player, string eventKey, int value)
		{
			if (player.DbModel == null || player.DbModel.Team < 1) return;

			var team = await _teamService.GetTeam(player.DbModel.Team);
			if (team == null) return;

			if (player.DbModel.Money < value)
			{
				await player.ShowComponent("Team", false);
				await player.Notify("FRAKTION", "Du hast nicht so viel Geld.", NotificationType.ERROR);
				return;
			}

			var date = DateTime.Now;
			var hour = date.Hour < 10 ? "0" + date.Hour : date.Hour.ToString();
			var minute = date.Minute < 10 ? "0" + date.Minute : date.Minute.ToString();
			var day = date.Day < 10 ? "0" + date.Day : date.Day.ToString();
			var month = date.Month < 10 ? "0" + date.Month : date.Month.ToString();

			team.BankHistory.Insert(0, new TeamClientBankData(player.Name, value, $"{hour}:{minute} {day}.{month}.{date.Year}", false));
			team.Money += value;
			await player.RemoveMoney(value);
			await player.Notify("FRAKTION", $"Du hast {value} in die Fraktionsbank eingezahlt.", NotificationType.SUCCESS);
		}

		private async void TakeMoney(ClPlayer player, string eventKey, int value)
		{
			if (player.DbModel == null || player.DbModel.Team < 1) return;

			var team = await _teamService.GetTeam(player.DbModel.Team);
			if (team == null) return;

			if(team.Money < value)
			{
				await player.ShowComponent("Team", false);
				await player.Notify("FRAKTION", "Die Fraktionsbank hat nicht so viel Geld.", NotificationType.ERROR);
				return;
			}

			var date = DateTime.Now;
			var day = date.Day < 10 ? "0" + date.Day : date.Day.ToString();
			var month = date.Month < 10 ? "0" + date.Month : date.Month.ToString();

			team.BankHistory.Insert(0, new TeamClientBankData(player.Name, value, $"{date.Hour}:{date.Minute} {day}.{month}.{date.Year}", true));
			team.Money -= value;
			await player.AddMoney(value);
			await player.Notify("FRAKTION", $"Du hast {value} von der Fraktionsbank abgehoben.", NotificationType.SUCCESS);
		}

		private async void KickMember(ClPlayer player, string eventKey, int id, bool fromPhone = false)
		{
			if (player.DbModel == null || player.DbModel.Team < 1 || !player.DbModel.TeamAdmin)
			{
				if(!fromPhone) await player.ShowComponent("Team", false);
				await player.Notify("FRAKTION", "Es ist ein Fehler aufgetreten!", NotificationType.ERROR);
				return;
			}

			if(player.DbModel.Id == id)
			{
				if (!fromPhone) await player.ShowComponent("Team", false);
				await player.Notify("FRAKTION", "Du kannst dich nicht selbst rauswerfen!", NotificationType.ERROR);
				return;
			}

			var target = _accountService.GetAccount(id);
			if (target == null)
			{
				if (!fromPhone) await player.ShowComponent("Team", false);
				await player.Notify("FRAKTION", "Es ist ein Fehler aufgetreten!", NotificationType.ERROR);
				return;
			}

			target.Team = 0;
			target.TeamRank = 0;
			target.TeamAdmin = false;
			target.TeamBankPermission = false;
			target.TeamStoragePermission = false;

			foreach (var teamMember in ClPlayer.All.Where(x => x.DbModel != null && x.DbModel.Team == player.DbModel.Team))
				await teamMember.Notify("FRAKTION", $"{player.Name} hat {target.Name} aus der Fraktion entlassen!", NotificationType.INFO);

			var targetPlayer = ClPlayer.All.FirstOrDefault(x => x.DbModel != null && x.DbModel.Id == id);
			if (targetPlayer == null) return;

			await targetPlayer.Notify("FRAKTION", $"Du wurdest von {player.Name} aus der Fraktion entlassen!", NotificationType.INFO);
			await targetPlayer.SetTeam(0);
		}

		private async void BuyEquip(ClPlayer player, string eventKey)
		{
			if (player.DbModel == null || player.DbModel.Team < 1 || (player.DbModel.Team < 6 && !player.DbModel.Duty)) return;

			if(player.DbModel.Money < 4500)
			{
				await player.Notify("FRAKTION", "Du hast nicht genug Geld!", NotificationType.ERROR);
				return;
			}

			var team = await _teamService.GetTeam(player.DbModel.Team);
			if (team == null) return;

			var pistol = (uint)WeaponModel.PistolMkII;

			if (player.DbModel.Loadout.FirstOrDefault(x => InventoryModule.ItemModels.FirstOrDefault(e => e.Type == ItemType.WEAPON && ((WeaponItemBase)e).WeaponType == WeaponType.PISTOL && x.Hash == ((WeaponItemBase)e).Hash) != null || x.Hash == team.MeeleWeaponHash) != null)
			{
				await player.Notify("FRAKTION", "Du hast bereits ein Teil des Equips dabei!", NotificationType.ERROR);
				return;
			}

			await player.RemoveMoney(4500);
			await player.AddWeapon(pistol, 224, new List<uint>(), 0);
			await player.AddWeapon(team.MeeleWeaponHash, 0, new List<uint>(), 0);
			await player.Notify("FRAKTION", "Du hast dein Fraktionsequipment für $4500 gekauft.", NotificationType.SUCCESS);
		}

		private async void UpdateMember(ClPlayer player, string eventKey, int targetId, int rank, bool admin, bool storage, bool bank)
		{
			if (player.DbModel == null || player.DbModel.Team < 1 || !player.DbModel.TeamAdmin)
			{
				await player.ShowComponent("Team", false);
				await player.Notify("FRAKTION", "Es ist ein Fehler aufgetreten!", NotificationType.ERROR);
				return;
			}

			if(player.DbModel.Id == targetId)
			{
				await player.ShowComponent("Team", false);
				await player.Notify("FRAKTION", "Du kannst dich nicht selbst bearbeiten!", NotificationType.ERROR);
				return;
			}

			var target = _accountService.GetAccount(targetId);
			if(target == null || target.Team != player.DbModel.Team)
			{
				await player.ShowComponent("Team", false);
				await player.Notify("FRAKTION", "Es ist ein Fehler aufgetreten!", NotificationType.ERROR);
				return;
			}

			if (target.TeamRank >= player.DbModel.TeamRank)
			{
				await player.ShowComponent("Team", false);
				await player.Notify("FRAKTION", "Es ist ein Fehler aufgetreten!", NotificationType.ERROR);
				return;
			}

			target.TeamRank = rank;
			target.TeamAdmin = admin;
			target.TeamStoragePermission = storage;
			target.TeamBankPermission = bank;

			foreach(var teamMember in ClPlayer.All.Where(x => x.DbModel != null && x.DbModel.Team == player.DbModel.Team))
				await teamMember.Notify("FRAKTION", $"{player.Name} hat {target.Name} auf Rang {rank} gesetzt!", NotificationType.INFO);
		}

		private async void OpenTeamMenu(ClPlayer player, string eventKey)
		{
			if (player.DbModel == null || player.DbModel.Team < 1 || player.CurrentShape?.ShapeType != ColshapeType.TEAM || player.DbModel.Team != player.CurrentShape?.Id) return;

			var team = await _teamService.GetTeam(player.DbModel.Team);
			if(team == null) return;

			if(team.Type == TeamType.POLICE) await OpenFedealTeamMenu(player, team);
			else await OpenCrimeTeamMenu(player, team);
		}

		private async Task OpenFedealTeamMenu(ClPlayer player, TeamModel team)
		{
			var members = new List<TeamClientMemberData>();

			foreach (var account in _accountService.Accounts.Where(x => x.Team == player.DbModel.Team))
				members.Add(new TeamClientMemberData(
					account.Id,
					account.Name,
					account.TeamRank,
					ClPlayer.All.FirstOrDefault(x => x.DbModel?.Id == account.Id) != null,
					account.TeamAdmin,
					account.TeamStoragePermission,
					account.TeamBankPermission,
					$"{account.LastLogin.Day}.{account.LastLogin.Month}.{account.LastLogin.Year}",
					$"{account.TeamJoinDate.Day}.{account.TeamJoinDate.Month}.{account.TeamJoinDate.Year}"));

			var teamData = new ClientFederalTeam(
				team.Id,
				team.Name,
				team.Warns,
				PoliceModule.SWATState,
				team.Money,
				members,
				team.BankHistory,
				new TeamClientBusinessData(2, 1, 2));

			await player.ShowComponent("Team", true, JsonConvert.SerializeObject(new TeamFederalClientData(
				player.Name,
				team.MeeleWeapon,
				player.DbModel.TeamAdmin,
				player.DbModel.TeamBankPermission,
				player.DbModel.Duty,
				player.DbModel.TeamStoragePermission,
				teamData)));
		}

		private async Task OpenCrimeTeamMenu(ClPlayer player, TeamModel team)
		{
			var gws = _gangwarService.Gangwar.Where(x => x.OwnerId == team.Id).Count();

			var slots = team.Inventories.Sum(x => x.Inventory.Slots);
			var freeSlots = team.Inventories.Sum(x => x.Inventory.GetFreeSlots());
			var weight = (int)Math.Ceiling(team.Inventories.Sum(x => x.Inventory.GetInventoryWeight()));
			var maxWeight = (int)team.Inventories.Sum(x => x.Inventory.MaxWeight);
			var drugs = team.Inventories.Sum(x => x.Inventory.Items.Where(x => x.Model.Id == team.Drug).Sum(x => x.Amount));

			var storageData = new TeamStorageClientData(
				team.Inventories.Count,
				weight,
				maxWeight,
				slots - freeSlots,
				slots,
				drugs);

			var members = new List<TeamClientMemberData>();

			foreach (var account in _accountService.Accounts.Where(x => x.Team == player.DbModel.Team))
				members.Add(new TeamClientMemberData(
					account.Id,
					account.Name,
					account.TeamRank,
					ClPlayer.All.FirstOrDefault(x => x.DbModel?.Id == account.Id) != null,
					account.TeamAdmin,
					account.TeamStoragePermission,
					account.TeamBankPermission,
					$"{account.LastLogin.Day}.{account.LastLogin.Month}.{account.LastLogin.Year}",
					$"{account.TeamJoinDate.Day}.{account.TeamJoinDate.Month}.{account.TeamJoinDate.Year}"));

			var teamData = new ClientTeam(
				team.Id,
				team.Name,
				team.Warns,
				gws,
				team.Money,
				storageData,
				members,
				team.BankHistory,
				new TeamClientLaboratoryData(team.Laboratory, (team.LabFuel.Items.Where(x => x.Model.Id == 501).Sum(x => x.Amount) / 480) * 100),
				new TeamClientBusinessData(2, 1, 2));

			var activeGangwar = await _gangwarController.GetRunningGangwarByTeam(player.DbModel.Team);
			TeamClientGangwarData? gwData = null;
			if (activeGangwar != null)
			{
				var enemy = await _teamService.GetTeam(activeGangwar.AttackerId == player.DbModel.Team ? activeGangwar.OwnerId : activeGangwar.AttackerId);
				gwData = new TeamClientGangwarData(activeGangwar.Name, enemy!.Name);
			}

			await player.ShowComponent("Team", true, JsonConvert.SerializeObject(new TeamClientData(
				player.Name,
				team.MeeleWeapon,
				player.DbModel.TeamAdmin,
				player.DbModel.TeamBankPermission,
				teamData,
				gwData)));
		}

		public Task OnColshape(ClShape shape, IEntity entity, bool entered)
		{
			if (entity.Type != BaseObjectType.Player) return Task.CompletedTask;

			var player = (ClPlayer)entity;

			switch (shape.ShapeType)
			{
				case ColshapeType.TEAM:
					if (entered) player.SetInteraction(Interactions.KEY_E, Interactions.E_TEAM);
					break;
				case ColshapeType.LABORATORY_INTERACTION:
					player.SetInteraction(Interactions.KEY_E, entered ? Interactions.E_LAB : Interactions.NONE);
					break;
			}
			return Task.CompletedTask;
		}

		public async Task OnEveryTenMinute()
		{
			foreach(var player in ClPlayer.All.Where(x => x.DbModel != null && x.LaboratoryStatus))
			{
				var team = await _teamService.GetTeam(player.DbModel.Team);
				if (team == null || team.LabFuel.Items.Where(x => x.Model.Id == 501).Sum(x => x.Amount) < 1) continue;

				var fuelModel = InventoryModule.GetItemBase(501)!;
				var drugModel = InventoryModule.GetItemBase(team.Drug)!;
				var utilModel = InventoryModule.GetItemBase(8)!;

				var hasDrugItems = player.DbModel.LaboratoryInput.HasItems(drugModel, 12);
				var hasUtilItems = player.DbModel.LaboratoryInput.HasItems(utilModel, 1);
				if(!hasDrugItems || !hasUtilItems || player.DbModel.LaboratoryOutput.GetFreeSlots() < 1)
				{
					player.LaboratoryStatus = false;
					continue;
				}

				player.DbModel.LaboratoryInput.RemoveItem(drugModel, 12);
				player.DbModel.LaboratoryInput.RemoveItem(utilModel, 1);
				player.DbModel.LaboratoryOutput.AddItem(InventoryModule.GetItemBase(team.ProcessedDrug)!, 1);
				team.LabFuel.RemoveItem(fuelModel, 1);
			}

			_teamService.Teams.ForEach(async x => await _teamService.UpdateTeam(x));
		}
	}
}