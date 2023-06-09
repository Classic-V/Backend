using Backend.Controllers.Event.Interface;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Phone;
using Newtonsoft.Json;
using Backend.Utils.Enums;
using Backend.Utils.Models.Input;
using Backend.Services.Team.Interface;

namespace Backend.Modules.Phone
{
	public class PhoneModule : Module<PhoneModule>
	{
		private readonly ITeamService _teamService;

		public PhoneModule(IEventController eventController, ITeamService teamService) : base("Phone")
		{
			_teamService = teamService;

			eventController.OnClient<string>("Server:Phone:Team:Invite", TeamInvitePlayer);
			eventController.OnClient("Server:Phone:Team:RequestData", RequestTeamData);
			eventController.OnClient("Server:Phone:Open", Open);
		}

		#region Team App
		private async void TeamInvitePlayer(ClPlayer player, string eventKey, string targetName)
		{
			if (player.DbModel == null || player.DbModel.Team < 1 || !player.DbModel.TeamAdmin) return;

			var target = ClPlayer.All.FirstOrDefault(x => x.DbModel != null && x.Name.ToLower() == targetName.ToLower());
			if(target == null)
			{
				await player.Notify("FRAKTION", $"Der Spieler {targetName} konnte nicht gefunden werden!", NotificationType.ERROR);
				return;
			}

			if(player.Position.Distance(target.Position) > 10)
			{
				await player.Notify("FRAKTION", $"Der Spieler muss in der Nähe sein!", NotificationType.ERROR);
				return;
			}

			if(target.DbModel.Team > 0)
			{
				await player.Notify("FRAKTION", $"Der Spieler {targetName} ist bereits in einer Fraktion!", NotificationType.ERROR);
				return;
			}

			var team = await _teamService.GetTeam(player.DbModel.Team);
			if(team == null)
			{
				await player.Notify("FRAKTION", $"Ein Fehler ist aufgetreten!", NotificationType.ERROR);
				return;
			}

			await target.ShowComponent("Input", true, JsonConvert.SerializeObject(new InputDataModel(
				"Fraktionseinladung",
				$"Du wurdest von {player.Name} in die Fraktion {team.Name} eingeladen!",
				InputType.CONFIRM,
				"Server:Team:AcceptInvite",
				player.DbModel.Team)));
		}
		private void RequestTeamData(ClPlayer player, string eventKey)
		{
			if (player.DbModel == null) return;

			var data = new List<PhoneTeamMemberData>();
			foreach(var target in ClPlayer.All)
			{
				if (target.DbModel == null || target.DbModel.Team != player.DbModel.Team) continue;

				data.Add(new(target.DbModel.Id, target.Name, target.DbModel.Id, target.DbModel.TeamRank, target.DbModel.TeamAdmin));
			}

			player.EmitBrowser("Phone:Team:SetData", JsonConvert.SerializeObject(data));
		}
		#endregion

		private void Open(ClPlayer player, string eventKey)
		{
			if (player.DbModel == null || !player.DbModel.Phone) return;

			player.PlayAnimation(AnimationType.PHONE);
			player.ShowComponent("Phone", true, JsonConvert.SerializeObject(new ClientPhoneData(
				player.DbModel.PhoneBackground,
				player.DbModel.Team,
				player.DbModel.TeamAdmin,
				player.DbModel.Business,
				player.DbModel.BusinessAdmin)));
		}
	}
}