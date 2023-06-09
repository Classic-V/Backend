using Backend.Controllers.JeweleryHeist.Interface;
using Backend.Utils.Models.Entities;
using Backend.Utils.Enums;
using Backend.Services.Team.Interface;
using Backend.Controllers.Team.Interface;
using Backend.Controllers.Team;

namespace Backend.Utils.Models.Inventory.Items
{
	public class HackingKit : ItemBase
	{
		public override int Id => 7;

		private readonly IJeweleryHeistController _jeweleryHeistController;
		private readonly ITeamController _teamController;

		private readonly ITeamService _teamService;

		public HackingKit(IJeweleryHeistController jeweleryHeistController, ITeamService teamService, ITeamController teamController) : base("Hacking Kit", 3f, 1)
		{
			_jeweleryHeistController = jeweleryHeistController;
			_teamController = teamController;

			_teamService = teamService;
		}

		public override async Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			var shape = player.CurrentShape;
			if (shape == null || player.DbModel.Team < 1) return false;

			switch (shape.ShapeType)
			{
				case ColshapeType.JEWELERY_MAIN: return await RobJewelery(player, slot, amount);
				case ColshapeType.LABORATORY_ROB: return await RobLaboratory(player, slot, amount, shape);
			}

			return true;
		}

		private async Task<bool> RobLaboratory(ClPlayer player, int slot, int amount, ClShape shape)
		{
			var team = await _teamService.GetTeam(shape.Id);
			if (team == null || team.Id == player.DbModel.Team || team.HasBeenRobbed) return false;

			await _teamController.RobLaboratory(player);

			return true;
		}

		private async Task<bool> RobJewelery(ClPlayer player, int slot, int amount)
		{
			if (_jeweleryHeistController.IsBeingRobbed || !await _jeweleryHeistController.StartHeist(player)) return false;

			await player.StartInteraction(async () =>
			{
				if (_jeweleryHeistController.IsBeingRobbed) return;

				await _jeweleryHeistController.UnlockLoot(player);
			}, 3000/*00*/);

			return true;
		}
	}
}