using Backend.Controllers.Event.Interface;
using Backend.Controllers.Storage.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;

namespace Backend.Modules.Chat.Public.Player
{
	public class StorageCommandModule : CommandModule<StorageCommandModule>
	{
		private readonly IStorageController _storageController;

		public StorageCommandModule(IEventController eventController, IStorageController storageController) : base("StorageCommand", AdminRank.PLAYER)
		{
			_storageController = storageController;

			eventController.OnClient("Server:Command:buystorage", BuyStorage);
		}

		private void BuyStorage(ClPlayer player, string eventKey)
		{
			if (!CheckPermission(player) || player.CurrentShape == null || player.CurrentShape.ShapeType != ColshapeType.JUMP_POINT || !player.CurrentShape.JumpPointType) return;

			_storageController.BuyStorage(player, player.CurrentShape.Id - 300000);
		}
	}
}