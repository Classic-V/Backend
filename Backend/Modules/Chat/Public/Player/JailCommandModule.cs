using Backend.Controllers.Event.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;

namespace Backend.Modules.Chat.Public.Player
{
	public class JailCommandModule : CommandModule<JailCommandModule>
	{
		public JailCommandModule(IEventController eventController) : base("JailCommand", AdminRank.PLAYER)
		{
			eventController.OnClient("Server:Command:jailtime", JailTime);
		}

		private async void JailTime(ClPlayer player, string eventKey)
		{
			if (!CheckPermission(player) || player.DbModel.Jailtime < 1) return;

			await player.Notify("Jailtime", $"Du bist noch für {player.DbModel.Jailtime} Minuten im Gefängnis!", NotificationType.INFO);
		}
	}
}