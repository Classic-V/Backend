using Backend.Utils.Models;
using Backend.Utils.Enums;
using Backend.Utils.Models.Entities;
using Backend.Controllers.Event.Interface;
using Backend.Services.Team.Interface;
using AltV.Net;
using Backend.Services.Account.Interface;

namespace Backend.Modules.Chat.Public.Admin
{
	public class AdminCommandModule : CommandModule<AdminCommandModule>
	{
		private readonly ITeamService _teamService;
		private readonly IAccountService _accountService;

		public AdminCommandModule(IEventController eventController, ITeamService teamService, IAccountService accountService) : base("AdminCommand", AdminRank.ADMINISTRATOR)
		{
			_teamService = teamService;
			_accountService = accountService;

			eventController.OnClient<string>("Server:Command:veh", SpawnVehicle);
			eventController.OnClient<string, int, int, int, int, int>("Server:Command:setteam", SetTeam);
			eventController.OnClient<string, int, int, int>("Server:Command:setfood", SetFood);
		}

		private async void SpawnVehicle(ClPlayer player, string eventKey, string name)
		{
			if (!CheckPermission(player)) return;

			try
			{
                var hash = Alt.Hash(name);
                if (hash == 0)
                {
                    await player.Notify("Administration", "Das angegebene Fahrzeug konnte nicht gefunden werden!", NotificationType.ERROR);
                    return;
                }

                var veh = new ClVehicle(Alt.Core, hash, player.Position, player.Rotation, null, 1000);
                
                if (!veh.Exists) return;
                
                await veh.SetEngineState(true);
                await veh.SetLockState(false);
                await veh.SetFuel(1000);
                
                player.SetIntoVehicle(veh, 1);
            } catch (Exception exception) {}
		}

		private async void SetTeam(ClPlayer player, string eventKey, string targetName = "", int teamId = -1, int rank = 0, int leader = 0, int storage = 0, int bank = 0)
		{
			if (!CheckPermission(player)) return;

			var account = _accountService.GetAccount(targetName);
			if (account == null)
			{
				await player.Notify("Administration", "Der Spieler konnte nicht gefunden werden.", NotificationType.ERROR);
				return;
			}
			
			var team = await _teamService.GetTeam(teamId);
			if (team == null)
			{
				await player.Notify("Administration", "Das Team konnte nicht gefunden werden.", NotificationType.ERROR);
				return;
			}
			
			account.Team = teamId;
			account.TeamRank = rank;
			account.TeamAdmin = Convert.ToBoolean(leader);
			account.TeamStoragePermission = Convert.ToBoolean(storage);
			account.TeamBankPermission = Convert.ToBoolean(bank);
			account.TeamJoinDate = DateTime.Now;
			
			await _accountService.UpdateAccount(account);
			
			await player.Notify("Administration", $"Das Team von {targetName} wurde auf {team.Name} geändert.", NotificationType.SUCCESS);
			
			var target = ClPlayer.All.FirstOrDefault(x => x.Name.ToLower() == targetName.ToLower());
			if (target != null)
			{
				await target.SetTeam(teamId);
			}
		}

		private async void SetFood(ClPlayer player, string eventKey, string targetName, int starvation, int hydration, int strength)
		{
			if (!CheckPermission(player)) return;

			var account = _accountService.GetAccount(targetName);
			if (account == null)
			{
				await player.Notify("Administration", "Der Spieler konnte nicht gefunden werden.", NotificationType.ERROR);
				return;
			}
			
			account.Starvation = starvation;
			account.Hydration = hydration;
			account.Strength = strength;
			
			var target = ClPlayer.All.FirstOrDefault(x => x.Name.ToLower() == targetName.ToLower());
			if (target != null)
			{
				await target.SetFood(starvation, hydration, strength);
			}
		}
	}
}