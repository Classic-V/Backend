using AltV.Net;
using AltV.Net.Async;
using Backend.Utils.Models;
using Backend.Utils.Enums;
using Backend.Controllers.Event.Interface;
using Backend.Utils.Models.Entities;
using AltV.Net.Data;
using Backend.Services.Team.Interface;
using Backend.Utils.Models.Player;

namespace Backend.Modules.Chat.Public.Admin
{
	public class SupporterCommandModule : CommandModule<SupporterCommandModule>
	{
		private readonly ITeamService _teamService;

		public SupporterCommandModule(IEventController eventController, ITeamService teamService) : base("SupporterCommand", AdminRank.SUPPORTER)
		{
			_teamService = teamService;

			eventController.OnClient<string>("Server:Command:respawn", Respawn);
			eventController.OnClient<string>("Server:Command:bring", Bring);
			eventController.OnClient<string>("Server:Command:goto", Goto);
			eventController.OnClient<string, string>("Server:Command:kick", Kick);
			eventController.OnClient<string>("Server:Command:revive", Revive);
			eventController.OnClient<string>("Server:Command:heal", Heal);
			eventController.OnClient<bool>("Server:Command:aduty", ToggleAduty);
			eventController.OnClient<int>("Server:Command:dv", DeleteVehicle);
			eventController.OnClient("Server:Command:tpm", GotoMarkerCommand);
			eventController.OnClient("Server:Command:vanish", Vanish);
			eventController.OnClient<string, string[]>("Server:Command:pm", PrivateMessage);
			eventController.OnClient<string>("Server:Command:freeze", Freeze);
			eventController.OnClient<float, float, float>("Server:Admin:SendMarkerPosition:ForGoto", GotoMarkerCallback);
		}

		private void Respawn(ClPlayer player, string eventKey, string targetName)
		{
			if (!CheckPermission(player)) return;

			var target = targetName == string.Empty ? player : ClPlayer.All.FirstOrDefault(x => x.DbModel != null && x.Name.ToLower() == targetName.ToLower());
			if (target == null)
			{
				player.Notify("Administration", "Der Spieler konnte nicht gefunden werden.", NotificationType.ERROR);
				return;
			}

			// airport
			var respawnPos = new Position(-1042.4308f, -2746.0483f, 21.343628f);

			if (target.DbModel.Team > 0)
			{
				var team = _teamService.GetTeam(target.DbModel.Team).Result;
				if (team != null) respawnPos = team.Position;
			}

			target.Spawn(player.Position, 0);
			target.SetHealth(200, 0, false);
			target.SetPosition(respawnPos);
			target.SetDimension(0);
			
			player.Notify("Administration", $"Du hast {target.Name} respawned.", NotificationType.INFO);
		}

		private void Bring(ClPlayer player, string eventKey, string targetName)
		{
			if (!CheckPermission(player)) return;

			var target = ClPlayer.All.FirstOrDefault(x => x.DbModel != null && x.Name.ToLower() == targetName.ToLower());
			if (target == null)
			{
				player.Notify("Administration", "Der Spieler konnte nicht gefunden werden.", NotificationType.ERROR);
				return;
			}

			target.SetPosition(player.Position);
		}

		private void Goto(ClPlayer player, string eventKey, string targetName)
		{
			if (!CheckPermission(player)) return;

			var target = ClPlayer.All.FirstOrDefault(x => x.DbModel != null && x.Name.ToLower() == targetName.ToLower());
			if (target == null)
			{
				player.Notify("Administration", "Der Spieler konnte nicht gefunden werden.", NotificationType.ERROR);
				return;
			}

			player.SetPosition(target.Position);
		}

		private void GotoMarkerCommand(ClPlayer player, string eventKey)
		{
			if (!CheckPermission(player)) return;
			
			player.Emit("Client:Admin:GetMarkerPosition:ForGoto");
		}
		
		private void GotoMarkerCallback(ClPlayer player, string eventKey, float x, float y, float z)
		{
			if (!CheckPermission(player)) return;
			
			player.SetPosition(new Position(x, y, z));
		}

		private void Kick(ClPlayer player, string eventKey, string targetName, string reason)
		{
			if (!CheckPermission(player)) return;

			var target = ClPlayer.All.FirstOrDefault(x => x.DbModel != null && x.Name.ToLower() == targetName.ToLower());
			if (target == null)
			{
				player.Notify("Administration", "Der Spieler konnte nicht gefunden werden.", NotificationType.ERROR);
				return;
			}

			target.Kick($"Du wurdest von {player.Name} gekicked! Grund: {reason}");
		}

		private void Revive(ClPlayer player, string eventKey, string targetName = "")
		{
			if (!CheckPermission(player)) return;

			var target = targetName == string.Empty ? player : ClPlayer.All.FirstOrDefault(x => x.DbModel != null && x.Name.ToLower() == targetName.ToLower());
			if (target == null)
			{
				player.Notify("Administration", "Der Spieler konnte nicht gefunden werden.", NotificationType.ERROR);
				return;
			}

			target.DbModel.Alive = true;
			target.DbModel.IsKoma = false;
			target.InHostpital = false;

			if (!target.Aduty) target.SetGodmode(false);
			
			target.Emit("Client:PlayerModule:SetAlive", true);
			target.SetStreamSyncedMetaData("ALIVE", true);
			target.SetStabilized(false);
			target.StopAnimation();
			target.SetHealth(200, 0);
			
			player.Notify("Administration", $"Du hast {target.Name} wiederbelebt.", NotificationType.SUCCESS);
		}
		
		private void Heal(ClPlayer player, string eventKey, string targetName = "")
		{
			if (!CheckPermission(player)) return;

			var target = targetName == string.Empty ? player : ClPlayer.All.FirstOrDefault(x => x.DbModel != null && x.Name.ToLower() == targetName.ToLower());
			if (target == null)
			{
				player.Notify("Administration", "Der Spieler konnte nicht gefunden werden.", NotificationType.ERROR);
				return;
			}

			target.SetHealth(200, 0);
			target.SetFood(100, 100, 100);
			
			player.Notify("Administration", $"Du hast {target.Name} geheilt.", NotificationType.SUCCESS);
		}
		
		private async void ToggleAduty(ClPlayer player, string eventKey, bool secret = false)
		{
			if (player.DbModel == null || player.DbModel.AdminRank < Utils.Enums.AdminRank.SUPPORTER) return;

			player.Aduty = !player.Aduty;

			await player.SetGodmode(player.Aduty);
			player.Emit("Client:AdminModule:SetDuty", player.Aduty);
			if (player.Aduty)
			{
				if(player.DbModel.Cuffed) await player.SetCuffed(false);
				if(player.DbModel.Roped) await player.SetRoped(false);
				if (!secret) await player.ApplyClothes((player.DbModel.Customization.Gender == 1 ? ClothesModel.MaleAdminClothes : ClothesModel.FemaleAdminClothes)[(int)player.DbModel.AdminRank]);
				player.Emit("nametags:Config", true, false, false, 100);
				await player.Notify("Administration", "Du bist nun im Admin-Duty.", NotificationType.INFO);
			} 
			else
			{
				await player.SetInvisible(true);
				await player.ApplyClothes();
				player.Emit("nametags:Config", false, false, false, 100);
				player.Notify("Administration", "Du bist nun nicht mehr im Admin-Duty.", NotificationType.INFO);
			}
		}

		private async void DeleteVehicle(ClPlayer player, string eventKey, int radius = 5)
		{
			if (!CheckPermission(player)) return;

			foreach (var vehicle in ClVehicle.All)
			{
				if (vehicle.Position.Distance(player.Position) <= radius)
				{
					vehicle.Destroy();
				}
			}		
		}
		
		private async void Vanish(ClPlayer player, string eventKey)
		{
			if (!CheckPermission(player)) return;
			if (!player.Aduty)
			{
				await player.Notify("Administration", "Du musst im Admin-Duty sein, um diesen Befehl zu nutzen.", NotificationType.ERROR);
				return;
			}

			player.Vanish = !player.Vanish;
			player.Emit("Client:AdminModule:SetVanish", player.Vanish);
			await player.SetInvisible(!player.Vanish);
		}

		private async void PrivateMessage(ClPlayer player, string eventKey, string targetName, params string[] message) 
		{
			if (!CheckPermission(player)) return;
			
			var target = ClPlayer.All.FirstOrDefault(x => x.Name.ToLower() == targetName.ToLower());
			if (target == null)
			{
				await player.Notify("Administration", "Der Spieler konnte nicht gefunden werden.", NotificationType.ERROR);
				return;
			}

			await target.Notify($"Administartion ({player.Name})", string.Join(" ", message), NotificationType.INFO);
		}

		private async void Freeze(ClPlayer player, string eventKey, string targetName)
		{
			if (!CheckPermission(player)) return;
			
			var target = ClPlayer.All.FirstOrDefault(x => x.Name.ToLower() == targetName.ToLower());
			if (target == null)
			{
				await player.Notify("Administration", "Der Spieler konnte nicht gefunden werden.", NotificationType.ERROR);
				return;
			}

			target.Frozen = !target.Frozen;
			await target.Freeze(target.Frozen);
			await player.Notify("Administration", $"Du hast {target.Name} " + (target.Frozen ? "gefreezed" : "unfreezed") + ".", NotificationType.SUCCESS);
		}
	}
}