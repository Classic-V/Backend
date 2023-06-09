using Backend.Utils.Models;
using Backend.Utils.Enums;
using Backend.Controllers.Event.Interface;
using Backend.Utils.Models.Entities;
using AltV.Net.Data;
using Backend.Services.Team.Interface;

namespace Backend.Modules.Chat.Public.Admin
{
	public class TeamCommandModule : CommandModule<TeamCommandModule>
	{
		private readonly ITeamService _teamService;

		public TeamCommandModule(IEventController eventController, ITeamService teamService) : base("TeamCommand", AdminRank.GUIDE)
		{
			_teamService = teamService;

			eventController.OnClient<string>("Server:CommandFulltext:teamchat", TeamChat);
			eventController.OnClient<string>("Server:CommandFulltext:tc", TeamChat);
		}
		
		private void TeamChat(ClPlayer player, string eventKey, string msg)
		{
			if (!CheckPermission(player)) return;

			ClPlayer.All.FindAll(target => target.DbModel.AdminRank >= AdminRank.GUIDE).ForEach(target =>
			{
				target.Notify("Teamchat (" + player.Name + ")", msg, NotificationType.INFO, ((msg.Split(" ").Length / 5) + 4) * 1000);
			});
		}
	}
}