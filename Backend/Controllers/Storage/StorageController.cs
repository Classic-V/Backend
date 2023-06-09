using System.Numerics;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Backend.Controllers.JumpPoint.Interface;
using Backend.Controllers.Storage.Interface;
using Backend.Services.JumpPoint.Interface;
using Backend.Services.Storage.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Inventory;
using Backend.Utils.Models.JumpPoint;
using Backend.Utils.Streamer;

namespace Backend.Controllers.Storage
{
    public class StorageController : IStorageController
    {
        private readonly IStorageService _storageService;
        private readonly IJumpPointController _jumpPointController;
        private readonly IJumpPointService _jumpPointService;

        public StorageController(IStorageService storageService, IJumpPointController jumpPointController, IJumpPointService jumpPointService)
        {
            _storageService = storageService;
            _jumpPointController = jumpPointController;
            _jumpPointService = jumpPointService;

            _storageService.Storages.ForEach(LoadStorage);
        }

        public async void LoadStorage(StorageModel storage)
        {
            storage.Inventories.ForEach(inventory => LoadInventory(storage, inventory));

			var upgrade = (ClShape)Alt.CreateColShapeSphere(GetUpgradePosition(storage.WarehouseType), 1f);
			upgrade.Id = storage.Id;
			upgrade.ShapeType = ColshapeType.STORAGE_UPGRADE;
			upgrade.Size = 1f;
			upgrade.Dimension = storage.Id;

			await _jumpPointController.CreateJumpPoint(
				new JumpPointModel(300000 + storage.Id,
					new JumpPointPosition(storage.Position, Rotation.Zero, 1.2f, 0),
					new JumpPointPosition(GetExitPosition(storage.WarehouseType), Rotation.Zero, 0.7f, storage.Id),
                    new List<int>() { storage.OwnerId }, new List<int>(), "")
                {
                    Name = "Lagerhalle"
                }, false);

			//var blip = await AltAsync.CreateBlip(BlipType.Destination, storage.Position);
			//blip.Name = "Lagerhalle";
			//blip.Sprite = 478;
			//blip.ShortRange = true;
			//blip.Color = 4;
			//blip.ScaleXY = new Vector2(1f, 1f);
		}

		private Position GetExitPosition(WarehouseType type)
		{
			return type == WarehouseType.SMALL_WAREHOUSE ? StorageModel.SmallExitPosition : type == WarehouseType.MEDIUM_WAREHOUSE ? StorageModel.MediumExitPosition : StorageModel.HighExitPosition;
		}

		private Position GetUpgradePosition(WarehouseType type)
		{
			return type == WarehouseType.SMALL_WAREHOUSE ? StorageModel.SmallUpgradePosition : type == WarehouseType.MEDIUM_WAREHOUSE ? StorageModel.MediumUpgradePosition : StorageModel.HighUpgradePosition;
		}

		public Task LoadInventory(StorageModel model, InventoryModel inventory)
		{
			var index = model.Inventories.IndexOf(inventory);
			var pos = new PositionModel();

			switch (model.WarehouseType)
			{
				case WarehouseType.SMALL_WAREHOUSE:
					pos = StorageModel.SmallPositions[index];
					break;
				case WarehouseType.MEDIUM_WAREHOUSE:
					pos = StorageModel.MediumPositions[index];
					break;
				case WarehouseType.HIGH_WAREHOUSE:
					pos = StorageModel.HighPositions[index];
					break;
			}

			var shape = (ClShape)Alt.CreateColShapeSphere(pos.Position, 1.6f);
			shape.Id = model.Id;
			shape.ShapeType = ColshapeType.STORAGE;
			shape.Size = 1.6f;
			shape.Inventory = inventory;
			shape.Dimension = model.Id;
			shape.StorageInventoryId = index;

			ObjectStreamer.AddObject(new Utils.Models.Object(GetBoxModelHash(index), pos.Position - new Position(0, 0, 1f), new Rotation(0, 0, pos.H), false, false, model.Id));

			return Task.CompletedTask;
		}

		public Task AddInventory(StorageModel model)
        {
			var inventory = new InventoryModel(200, 20, InventoryType.STORAGE);
			model.Inventories.Add(inventory);

			var index = model.Inventories.IndexOf(inventory);

			var pos = new PositionModel();

			switch (model.WarehouseType)
			{
				case WarehouseType.SMALL_WAREHOUSE:
                    pos = StorageModel.SmallPositions[index];
					break;
				case WarehouseType.MEDIUM_WAREHOUSE:
					pos = StorageModel.MediumPositions[index];
					break;
				case WarehouseType.HIGH_WAREHOUSE:
					pos = StorageModel.HighPositions[index];
					break;
			}

			var shape = (ClShape)Alt.CreateColShapeSphere(pos.Position, 1.6f);
			shape.Id = model.Id;
			shape.ShapeType = ColshapeType.STORAGE;
			shape.Size = 1.6f;
			shape.Inventory = inventory;
			shape.Dimension = model.Id;
			shape.StorageInventoryId = index;

			ObjectStreamer.AddObject(new Utils.Models.Object(GetBoxModelHash(index), pos.Position - new Position(0, 0, 1f), new Rotation(0, 0, pos.H), false, false, model.Id));

			return Task.CompletedTask;
        }

		private uint GetBoxModelHash(int index)
		{
			return index % 3 == 0 ? 2107849419 : index % 2 == 0 ? 307713837 : 2433343420;
		}

		public async Task CreateStorage(StorageModel model)
        {
            await _storageService.AddStorage(model);

            LoadStorage(model);
        }

		public async Task Upgrade(StorageModel model)
		{
			lock (ClShape.All)
			{
				foreach (var shape in ClShape.All.Where(x => x.Id == model.Id && x.ShapeType == ColshapeType.STORAGE).ToList())
					shape.Destory();
			}

			await _jumpPointService.RemoveLocalJumpPoint(model.Id + 300000);

			model.WarehouseType++;
			LoadStorage(model);
		}

		public async void BuyStorage(ClPlayer player, int storageId)
        {
            var storageData = await _storageService.GetStorage(storageId);
            if (storageData == null) return;

			if (storageData.OwnerId > 0) return;

            if(await _storageService.HasStorage(player.DbModel.Id))
            {
                await player.Notify("INFORMATION", "Du besitzt bereits eine Lagerhalle!", NotificationType.ERROR);
                return;
            }

            if(player.DbModel.BankMoney < 80000)
            {
				await player.Notify("INFORMATION", "Du hast nicht genug Geld! ($80000)", NotificationType.ERROR);
				return;
			}

			player.DbModel.BankMoney -= 80000;
			storageData.OwnerId = player.DbModel.Id;
            await _jumpPointController.SetOwner(storageId + 300000, new List<int>() { player.DbModel.Id }, new List<int>());

            await player.Notify("Lagerhalle", $"Du hast die Lagerhalle {storageData.Id} gekauft!", NotificationType.SUCCESS, 1000 * 5);
        }

        public async void SellStorage(ClPlayer player, int storageId, ClPlayer? buyer = null)
        {
            if (player == null) return;

            var storageData = await _storageService.GetStorage(storageId);
            if (storageData == null || storageData.OwnerId != player.DbModel.Id) return;

            var jumpPoint = await _jumpPointService.GetJumpPoint(300000 + storageData.Id);
            if (jumpPoint == null) return;

            jumpPoint.Locked = true;

            if (buyer == null)
            {
                storageData.OwnerId = 0;
                storageData.Inventories = new List<InventoryModel>();

                jumpPoint.PlayerAccessList[0] = 0;
                return;
            }

            storageData.OwnerId = buyer.DbModel.Id;
            jumpPoint.PlayerAccessList[0] = buyer.DbModel.Id;
        }
    }
}
