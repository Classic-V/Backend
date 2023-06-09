using Backend.Controllers.Event.Interface;
using Backend.Controllers.Storage.Interface;
using Backend.Services.Account.Interface;
using Backend.Services.Crime.Interface;
using Backend.Services.Storage.Interface;
using Backend.Services.Team.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Laptop;
using Discord;
using Newtonsoft.Json;

namespace Backend.Modules.Laptop
{
	public class LaptopModule : Module<LaptopModule>
	{
		public static readonly List<DispatchModel> Dispatches = new();
		public static readonly List<UnitModel> Units = new();

		private readonly IAccountService _accountService;
		private readonly ICrimeService _crimeService;
		private readonly ITeamService _teamService;
		private readonly IStorageService _storageService;

		public LaptopModule(IEventController eventController, IAccountService accountService, ICrimeService crimeService, ITeamService teamService, IStorageService storageService) : base("Laptop")
		{
			_accountService = accountService;
			_crimeService = crimeService;
			_teamService = teamService;
			_storageService = storageService;

			// CRIMES APP 
			eventController.OnClient<int, string>("Server:Laptop:Crimes:AddCrimes", AddCrimes);
			eventController.OnClient<int, string, string, int>("Server:Laptop:Crimes:SaveRecord", SaveRecord);
			eventController.OnClient<int>("Server:Laptop:Crimes:RequestPlayerData", GetPlayerData);
			eventController.OnClient<int, int>("Server:Laptop:Crimes:RemoveCrime", RemoveCrime);
			eventController.OnClient("Server:Laptop:Crimes:RequestCrimes", RequestCrimes);
			eventController.OnClient<string>("Server:Laptop:Crimes:Search", SearchRecords);

			// UNITS APP
			eventController.OnClient<int>("Server:Laptop:Units:CallUnit", CallUnit);
			eventController.OnClient<int>("Server:Laptop:Units:LocateUnit", LocateUnit);
			eventController.OnClient<int, int>("Server:Laptop:Units:UpdateVehicle", UpdateUnitVehicle);
			eventController.OnClient<int, int>("Server:Laptop:Units:RemovePlayer", RemoveUnitPlayer);
			eventController.OnClient<int, string>("Server:Laptop:Units:AddPlayer", AddUnitPlayer);
			eventController.OnClient<int>("Server:Laptop:Units:RemoveUnit", RemoveUnit);
			eventController.OnClient<string, int>("Server:Laptop:Units:AddUnit", AddUnit);
			eventController.OnClient("Server:Laptop:Units:RequestData", RequestUnitsData);

			// DISPATCH APP
			eventController.OnClient<int>("Server:Laptop:Dispatch:Accept", AcceptDispatch);
			eventController.OnClient<int>("Server:Laptop:Dispatch:Close", CloseDispatch);
			eventController.OnClient("Server:Laptop:Dispatch:RequestData", RequestDispatchData);

			eventController.OnClient("Server:Laptop:Open", Open);
		}

		#region Crimes App
		private bool IsAllowedToCrimeInteraction(ClPlayer player)
		{
			return player.DbModel != null && (player.DbModel.Team == 1 || player.DbModel.Team == 2 || player.DbModel.Team == 5);
		}

		private async void AddCrimes(ClPlayer player, string eventKey, int targetId, string json)
		{
			if (!IsAllowedToCrimeInteraction(player)) return;

			var target = _accountService.GetAccount(targetId);
			if (target == null) return;

			var team = await _teamService.GetTeam(player.DbModel.Team);
			if (team == null) return;

			var crimes = JsonConvert.DeserializeObject<List<int>>(json);
			if (crimes == null || crimes.Count < 1) return;

			var date = DateTime.Now;
			var dateLabel = $"{ConvertNumber(date.Hour)}:{ConvertNumber(date.Minute)} {ConvertNumber(date.Day)}.{ConvertNumber(date.Month)}.{date.Year}";

			crimes.ForEach(x => target.Crimes.Add(new Utils.Models.Player.PlayerCrimeModel(x, dateLabel, player.Name)));
			foreach (var federalPlayer in ClPlayer.All.Where(x => x.DbModel != null && (x.DbModel.Team == 1 || x.DbModel.Team == 2 || x.DbModel.Team == 5)))
				await federalPlayer.Notify("Information", $"{player.Name}({team.ShortName} R{player.DbModel.TeamRank}) hat für {target.Name} Akte erstellt!", NotificationType.INFO);
		}

		private string ConvertNumber(int number)
		{
			return number < 10 ? $"0{number}" : $"{number}";
		}

		private void SaveRecord(ClPlayer player, string eventKey, int targetId, string team, string description, int phone)
		{
			if (!IsAllowedToCrimeInteraction(player)) return;

			if (player.DbModel.TeamRank < 7)
			{
				player.Notify("Information", "Du kannst diese Daten erst ab Rang 7 anpassen!", NotificationType.INFO);
				return;
			}

			var target = _accountService.GetAccount(targetId);
			if (target == null) return;

			target.FederalRecordTeam = team;
			target.FederalRecordDescription = description;
			target.FederalRecordPhone = phone;

			player.Notify("Information", $"Du hast die Akte von {target.Name} bearbeitet!", NotificationType.INFO);
		}

		private void GetPlayerData(ClPlayer player, string eventKey, int targetId)
		{
			if (!IsAllowedToCrimeInteraction(player)) return;

			var target = _accountService.GetAccount(targetId);
			if (target == null) return;

			var team = _teamService.GetTeam(target.Team);
			var storage = _storageService.Storages.FirstOrDefault(x => x.OwnerId == target.Id);

			player.EmitBrowser("Laptop:Crimes:SetUserData", JsonConvert.SerializeObject(new PlayerDataModel(
				target.Id,
				target.Name,
				0,
				storage == null ? 0 : storage.Id,
				target.FederalRecordTeam,
				target.FederalRecordDescription,
				target.FederalRecordPhone,
				target.Crimes)));
		}

		private async void RemoveCrime(ClPlayer player, string eventKey, int targetId, int index)
		{
			if (!IsAllowedToCrimeInteraction(player)) return;

			var team = await _teamService.GetTeam(player.DbModel.Team);
			if (team == null) return;

			var target = _accountService.GetAccount(targetId);
			if (target == null) return;

			if (target.Crimes.Count <= index) return;

			var crime = _crimeService.Crimes.FirstOrDefault(x => x.Id == target.Crimes[index].CrimeId);
			if (crime == null) return;

			foreach (var federalPlayer in ClPlayer.All.Where(x => x.DbModel != null && (x.DbModel.Team == 1 || x.DbModel.Team == 2 || x.DbModel.Team == 5)))
				await federalPlayer.Notify("Information", $"{player.Name}({team.ShortName} R{player.DbModel.TeamRank}) hat {target.Name} {crime.Label} erlassen!", NotificationType.INFO);

			target.Crimes.RemoveAt(index);

			var targetPlayer = ClPlayer.All.FirstOrDefault(x => x.DbModel != null && x.DbModel.Id == targetId);
			if (targetPlayer == null) return;

			await targetPlayer.Notify("Information", "Dir wurde eine Akte erlassen!", NotificationType.INFO);
		}

		private void RequestCrimes(ClPlayer player, string eventKey)
		{
			if (!IsAllowedToCrimeInteraction(player)) return;

			player.EmitBrowser("Laptop:Crimes:SetCrimes", _crimeService.CrimesJSON);
		}

		private void SearchRecords(ClPlayer player, string eventKey, string search)
		{
			if (!IsAllowedToCrimeInteraction(player)) return;

			var list = _accountService.Accounts.Where(x => x.Name.ToLower().Contains(search.ToLower())).ToList();

			var targets = new List<CrimePlayerModel>();
			for(var i = 0; i < list.Count; i++)
			{
				if (i >= 9) break;
				targets.Add(new CrimePlayerModel(list[i].Id, list[i].Name));
			}

			player.EmitBrowser("Laptop:Crimes:SetPlayers", JsonConvert.SerializeObject(targets));
		}
		#endregion

		#region Units App
		private void CallUnit(ClPlayer player, string eventKey, int unitId)
		{
			var unit = Units.FirstOrDefault(x => x.Id == unitId);
			if (unit == null) return;

			unit.Players.ForEach(x =>
			{
				var target = ClPlayer.All.FirstOrDefault(e => e.DbModel != null && e.DbModel.Id == x.Id);
				if(target == null) return;

				target.Emit("Client:PlayerModule:SetWaypoint", player.Position.X, player.Position.Y);
				target.Notify("Fraktion", $"Deine Einheit wurde von {player.Name} angefordert!", NotificationType.INFO);
			});

			player.Notify("Inforamtion", $"Du hast Einheit {unit.Name} angefordert!", NotificationType.INFO);
		}

		private void LocateUnit(ClPlayer player, string eventKey, int unitId)
		{
			var unit = Units.FirstOrDefault(x => x.Id == unitId);
			if (unit == null) return;

			var veh = ClVehicle.All.FirstOrDefault(x => x.DbModel != null && x.DbModel.Id == unitId && (x.DbModel.Type == VehicleType.TEAM && x.DbModel.Owner == player.DbModel.Team || x.DbModel.Type == VehicleType.SWAT));
			if (veh == null)
			{
				player.Notify("Inforamtion", $"Die Einheit konnte nicht geortet werden!", NotificationType.ERROR);
				return;
			}

			player.Emit("Client:PlayerModule:SetWaypoint", veh.Position.X, veh.Position.Y);
			player.Notify("Inforamtion", $"Du hast Einheit {unit.Name} geortet!", NotificationType.INFO);
		}

		private void UpdateUnitVehicle(ClPlayer player, string eventKey, int unitId, int vehicleId)
		{
			var unit = Units.FirstOrDefault(x => x.Id == unitId);
			if (unit == null) return;

			unit.Vehicle = vehicleId;

			var unitData = JsonConvert.SerializeObject(unit);

			foreach (var teamMember in ClPlayer.All.Where(x => x.DbModel != null && x.DbModel.Team == player.DbModel.Team))
				teamMember.EmitBrowser("Laptop:Units:UpdateUnit", unitData);

			player.Notify("Inforamtion", $"Du hast das Fahrzeug von Einheit {unit.Name} geändert!", NotificationType.INFO);
		}

		private void RemoveUnitPlayer(ClPlayer player, string eventKey, int unitId, int targetId)
		{
			var unit = Units.FirstOrDefault(x => x.Id == unitId);
			if (unit == null) return;

			var target = unit.Players.FirstOrDefault(x => x.Id == targetId);
			if (target == null) return;

			unit.Players.Remove(target);

			var unitData = JsonConvert.SerializeObject(unit);

			foreach (var teamMember in ClPlayer.All.Where(x => x.DbModel != null && x.DbModel.Team == player.DbModel.Team))
				teamMember.EmitBrowser("Laptop:Units:UpdateUnit", unitData);

			player.Notify("Inforamtion", $"Du hast {target.Name} von Einheit {unit.Name} entfernt!", NotificationType.INFO);
		}

		private void AddUnitPlayer(ClPlayer player, string eventKey, int unitId, string targetInput)
		{
			ClPlayer? target = null;
			if(int.TryParse(targetInput, out var targetId))
			{
				target = ClPlayer.All.FirstOrDefault(x => x.DbModel != null && x.DbModel.Id == targetId && x.DbModel.Team == player.DbModel.Team);
			}
			else
			{
				target = ClPlayer.All.FirstOrDefault(x => x.DbModel != null && x.Name.ToLower() == targetInput.ToLower() && x.DbModel.Team == player.DbModel.Team);
			}

			if(target == null) return;

			var unit = Units.FirstOrDefault(x => x.Id == unitId);
			if (unit == null) return;

			unit.Players.Add(new UnitMemberModel(target.DbModel.Id, target.Name, target.DbModel.TeamRank));

			var unitData = JsonConvert.SerializeObject(unit);

			foreach (var teamMember in ClPlayer.All.Where(x => x.DbModel != null && x.DbModel.Team == player.DbModel.Team))
				teamMember.EmitBrowser("Laptop:Units:UpdateUnit", unitData);

			player.Notify("Inforamtion", $"Du hast {target.Name} zu Einheit {unit.Name} hinzugefügt!", NotificationType.INFO);
		}

		private void RemoveUnit(ClPlayer player, string eventKey, int unitId)
		{
			var unit = Units.FirstOrDefault(x => x.Id == unitId);
			if (unit == null) return;

			Units.Remove(unit);

			var unitData = JsonConvert.SerializeObject(unit);

			foreach (var teamMember in ClPlayer.All.Where(x => x.DbModel != null && x.DbModel.Team == player.DbModel.Team))
				teamMember.EmitBrowser("Laptop:Units:RemoveUnit", unitData);

			player.Notify("Inforamtion", $"Du hast Einheit {unit.Name} entfernt!", NotificationType.INFO);
		}

		private void AddUnit(ClPlayer player, string eventKey, string name, int vehicle)
		{
			var unit = new UnitModel(name, vehicle, new(), player.DbModel.Team);
			Units.Add(unit);

			var unitData = JsonConvert.SerializeObject(unit);

			foreach (var teamMember in ClPlayer.All.Where(x => x.DbModel != null && x.DbModel.Team == player.DbModel.Team))
				teamMember.EmitBrowser("Laptop:Units:UpdateUnit", unitData);

			player.Notify("Inforamtion", $"Du hast Einheit {unit.Name} erstellt!", NotificationType.INFO);
		}

		private void RequestUnitsData(ClPlayer player, string eventKey)
		{
			if (player.DbModel.Team < 1 || player.DbModel.Team > 5) return;

			player.EmitBrowser("Laptop:Units:SetData", JsonConvert.SerializeObject(Units.Where(x => x.Team == player.DbModel.Team)));
		}
		#endregion

		#region Dispatch App
		private void AcceptDispatch(ClPlayer player, string eventKey, int id)
		{
			var dispatch = Dispatches.FirstOrDefault(x => x.Id == id);
			if (dispatch == null) return;

			var creator = ClPlayer.All.FirstOrDefault(x => x.DbModel.Id == dispatch.CreatorId);
			if (creator != null) creator.Notify("Dispatch", "Dein Dispatch wurde angenommen!", NotificationType.INFO);

			foreach (var teamMember in ClPlayer.All.Where(x => x.DbModel != null && x.DbModel.Team == player.DbModel.Team))
				teamMember.Notify("Dispatch", $"{player.Name} hat den Dispatch von {dispatch.Creator} angenommen.", NotificationType.INFO);

			dispatch.Officer = player.Name;
		}

		private void CloseDispatch(ClPlayer player, string eventKey, int id)
		{
			var dispatch = Dispatches.FirstOrDefault(x => x.Id == id);
			if (dispatch == null) return;

			var creator = ClPlayer.All.FirstOrDefault(x => x.DbModel.Id == dispatch.CreatorId);
			if (creator != null) creator.Notify("Dispatch", "Dein Dispatch wurde geschlossen!", NotificationType.INFO);

			foreach (var teamMember in ClPlayer.All.Where(x => x.DbModel != null && x.DbModel.Team == player.DbModel.Team))
				teamMember.Notify("Dispatch", $"{player.Name} hat den Dispatch von {dispatch.Creator} geschlossen.", NotificationType.INFO);

			Dispatches.Remove(dispatch);
		}

		private void RequestDispatchData(ClPlayer player, string eventKey)
		{
			player.EmitBrowser("Laptop:Dispatch:SetData", JsonConvert.SerializeObject(Dispatches.Where(x => x.Type == GetTeamDispatchType(player.DbModel.Team))));
		}
		#endregion

		private void Open(ClPlayer player, string eventKey)
		{
			if (!player.DbModel.Laptop) return;

			player.PlayAnimation(AnimationType.LAPTOP);
			player.ShowComponent("Laptop", true, JsonConvert.SerializeObject(new
			{
				Name = player.Name,
				Team = player.DbModel.Team,
				TeamLeader = player.DbModel.TeamAdmin
			}));
		}

		private DispatchType GetTeamDispatchType(int team)
		{
			switch (team)
			{
				case 1: case 2: return DispatchType.LSPD;
				case 3: return DispatchType.LSMD;
				case 4: return DispatchType.DOPS;
			}

			return 0;
		}
	}
}