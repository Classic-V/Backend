using Backend.Controllers.Event.Interface;
using Backend.Controllers.House.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;

namespace Backend.Modules.Chat.Public.Player
{
	public class HouseCommandModule : CommandModule<HouseCommandModule>
	{
		private readonly IHouseController _houseController;

		public HouseCommandModule(IEventController eventController, IHouseController houseController) : base("HouseCommand", AdminRank.PLAYER)
		{
			_houseController = houseController;

			eventController.OnClient("Server:Command:buyhouse", BuyHouse);
		}

		private void BuyHouse(ClPlayer player, string eventKey)
		{
			if (!CheckPermission(player) || player.CurrentShape == null || player.CurrentShape.ShapeType != ColshapeType.HOUSE) return;

			_houseController.BuyHouse(player, player.CurrentShape.Id);
		}
	}
}