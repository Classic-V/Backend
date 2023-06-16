using Backend.Controllers.Event.Interface;
using Backend.Services.Account.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Player;
using Backend.Utils.Models.Player.Client;

namespace Backend.Modules.Admin
{
	public class AdminModule : Module<AdminModule>
	{
		private IAccountService _accountService;
		public AdminModule(IEventController eventController, IAccountService accountService) : base("Admin")
		{
			_accountService = accountService;
			eventController.OnClient<int, string>("Server:Admin:ResetId", ResetId);
			eventController.OnClient<string, string>("Server:Admin:Teleport", Teleport);
		}

		private async void ResetId(ClPlayer player, string eventKey, int targetId, string idType)
		{
			if (player.DbModel.AdminRank < AdminRank.MODERATOR) return;
			
			var targetAccount = _accountService.GetAccount(targetId);
			if (targetAccount == null)
			{
				await player.Notify("Administration", "Der Account konnte nicht gefunden werden.", NotificationType.ERROR);
				return;
			}
			
			switch (idType)
			{
				case "social":
					targetAccount.SocialClub = 0;
					await player.Notify("Administration", $"Die Socialclub-Id von {targetAccount.Name} wurde zurück gesetzt!", NotificationType.SUCCESS);
					break;
				case "dc":
					targetAccount.DiscordId = 0;
					await player.Notify("Administration", $"Die Discord-Id von {targetAccount.Name} wurde zurück gesetzt!", NotificationType.SUCCESS);
					break;
				case "hwid":
					targetAccount.HardwareId = 0;
					targetAccount.HardwareIdEx = 0;
					await player.Notify("Administration", $"Die Hardware-Id von {targetAccount.Name} wurde zurück gesetzt!", NotificationType.SUCCESS);
					break;
			}
			
			await _accountService.UpdateAccount(targetAccount);
		}

		private async void Teleport(ClPlayer player, string eventKey, string targetName, string teleportType)
		{
			if (player.DbModel.AdminRank < AdminRank.SUPPORTER) return;

			var target = ClPlayer.All.FirstOrDefault(x => x.Name.ToLower() == targetName.ToLower());
			if (target == null)
			{
				await player.Notify("Administration", "Der Spieler konnte nicht gefunden werden.", NotificationType.ERROR);
				return;
			}
			Console.WriteLine(teleportType);
			switch (teleportType)
			{
				case "to":
					player.Position = target.Position;
					await player.Notify("Administration", $"Du wurdest zu {target.Name} teleportiert!", NotificationType.SUCCESS);
					break;
				case "here":
					target.Position = player.Position;
					await player.Notify("Administration", $"{target.Name} wurde zu dir teleportiert!", NotificationType.SUCCESS);
					break;
			}
		}
	}
}