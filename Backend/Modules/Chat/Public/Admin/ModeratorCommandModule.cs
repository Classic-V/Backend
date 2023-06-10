using Backend.Controllers.Event.Interface;
using Backend.Services.Account.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Inventory;
using Backend.Utils.Models.Player;

namespace Backend.Modules.Chat.Public.Admin
{
	public class ModeratorCommandModule : CommandModule<ModeratorCommandModule>
	{
		private readonly IAccountService _accountService;

		public ModeratorCommandModule(IEventController eventController, IAccountService accountService) : base("ModeratorCommand", AdminRank.MODERATOR)
		{
			_accountService = accountService;

			eventController.OnClient<string>("Server:Command:resetdc", ResetDiscord);
			eventController.OnClient<string>("Server:Command:resetsocial", ResetSocial);
			eventController.OnClient<string>("Server:Command:resethwid", ResetHwid);
			eventController.OnClient<string, int>("Server:Command:setdimension", SetDimension);
			eventController.OnClient<string, string>("Server:Command:setname", SetName);
			eventController.OnClient<string>("Server:Command:clearinv", ClearInv);
			eventController.OnClient<string>("Server:Command:clearwep", ClearWeapons);
			eventController.OnClient("Server:Command:rep", Repair);
		}

		private void ResetDiscord(ClPlayer player, string eventKey, string targetName)
		{
			if (!CheckPermission(player)) return;

			var target = _accountService.GetAccount(targetName);
			if (target == null)
			{
				player.Notify("Administration", "Der Account konnte nicht gefunden werden.", NotificationType.ERROR);
				return;
			}

			target.DiscordId = 0;
			
			player.Notify("Administration", $"Die Discord-Id von {targetName} wurde zurück gesetzt!", NotificationType.SUCCESS);
		}

		private void ResetSocial(ClPlayer player, string eventKey, string targetName)
		{
			if (!CheckPermission(player)) return;

			var target = _accountService.GetAccount(targetName);
			if (target == null)
			{
				player.Notify("Administration", "Der Account konnte nicht gefunden werden.", NotificationType.ERROR);
				return;
			}

			target.SocialClub = 0;
			
			player.Notify("Administration", $"Die Socialclub-Id von {targetName} wurde zurück gesetzt!", NotificationType.SUCCESS);
		}

		private void ResetHwid(ClPlayer player, string eventKey, string targetName)
		{
			if (!CheckPermission(player)) return;

			var target = _accountService.GetAccount(targetName);		
			if (target == null)
			{
				player.Notify("Administration", "Der Account konnte nicht gefunden werden.", NotificationType.ERROR);
				return;
			}

			target.HardwareId = 0;
			target.HardwareIdEx = 0;
			
			player.Notify("Administration", $"Die Hardware-Id von {targetName} wurde zurück gesetzt!", NotificationType.SUCCESS);
		}

		private async void SetDimension(ClPlayer player, string eventKey, string targetName, int dimension)
		{
			if (!CheckPermission(player)) return;

			var target = ClPlayer.All.FirstOrDefault(x => x.Name.ToLower() == targetName.ToLower());
			if (target == null)
			{
				await player.Notify("Administration", "Der Spieler konnte nicht gefunden werden.", NotificationType.ERROR);
				return;
			}
			
			await target.SetDimension(dimension);
			await player.Notify("Administration", $"Die Dimension von {target.Name} wurde auf {dimension} gesetzt!", NotificationType.SUCCESS);
		}

		private async void SetName(ClPlayer player, string eventKey, string targetName, string newName)
		{
			if (!CheckPermission(player)) return;

			var acc = _accountService.GetAccount(targetName);
			if (acc == null)
			{
				await player.Notify("Administration", "Der Spieler konnte nicht gefunden werden.", NotificationType.ERROR);
				return;
			}

			acc.Name = newName;

			var target = ClPlayer.All.FirstOrDefault(x => x.Name.ToLower() == targetName.ToLower());
			if (target == null)
			{
				await _accountService.UpdateAccount(acc);
			}
			else
			{
				target.Kick("Du wurdest gekicked! Grund: Namensänderung, bitte das Spiel neustarten und den neuen Namen in den Alt:V Einstellungen eintragen.");
			}
			
			await player.Notify("Administration", $"Der Name von {targetName} wurde auf {newName} gesetzt!", NotificationType.SUCCESS);
		}
		
		public async void ClearInv(ClPlayer player, string eventKey, string targetName)
		{
			if (!CheckPermission(player)) return;

			var acc = _accountService.GetAccount(targetName);
			if (acc == null)
			{
				await player.Notify("Administration", "Der Spieler konnte nicht gefunden werden.", NotificationType.ERROR);
				return;
			}
			
			acc.Inventory = new InventoryModel(InventoryType.PLAYER);
			acc.Backpack = false;
			acc.Inventory.Slots = 8;
			acc.Inventory.MaxWeight = 45f;
			
			await _accountService.UpdateAccount(acc);
			
			await player.Notify("Administration", "Du hast das Inventar von " + acc.Name + " geleert!", NotificationType.SUCCESS);
		}
		
		public async void ClearWeapons(ClPlayer player, string eventKey, string targetName)
		{
			if (!CheckPermission(player)) return;

			var acc = _accountService.GetAccount(targetName);
			if (acc == null)
			{
				await player.Notify("Administration", "Der Spieler konnte nicht gefunden werden.", NotificationType.ERROR);
				return;
			}

			acc.Loadout = new List<LoadoutModel>();
			
			var target = ClPlayer.All.FirstOrDefault(x => x.Name.ToLower() == targetName.ToLower());
			if (target != null)
			{
				target.RemoveAllWeapons();
			}
			
			await player.Notify("Administration", "Du hast die Waffen von " + acc.Name + " geleert!", NotificationType.SUCCESS);
		}

		public async void Repair(ClPlayer player, string eventKey)
		{
			if (!CheckPermission(player)) return;

			if (player.IsInVehicle)
			{
				player.Vehicle.Repair();
				await player.Notify("Administration", "Das Fahrzeug wurde repariert!", NotificationType.SUCCESS);
			}
			else
			{
				await player.Notify("Administration", "Du bist in keinem Fahrzeug!", NotificationType.ERROR);
			}
		}
	}
}