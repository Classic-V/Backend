using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Autofac;
using Backend.Controllers.Event.Interface;
using Backend.Controllers.Storage.Interface;
using Backend.Data;
using Backend.Services.JumpPoint.Interface;
using Backend.Services.Storage.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Inventory;
using Backend.Utils.Models.Player.Client;
using Newtonsoft.Json;

namespace Backend.Modules.Storage
{
    public class StorageModule : Module<StorageModule>, IEventColshape
    {
		private readonly IStorageController _storageController;
        private readonly IStorageService _storageService;
        private readonly IJumpPointService _jumpPointService;

        public StorageModule(IStorageController storageController, IStorageService storageService, IJumpPointService jumpPointService, IEventController eventController) : base("Storage")
        {
            _storageController = storageController;
            _storageService = storageService;
            _jumpPointService = jumpPointService;

            eventController.OnClient("Server:Storage:Interact", OpenUpgrader);
			eventController.OnClient<int>("Server:Storage:ShowUpgrades", ShowUpgrades);
			eventController.OnClient<int>("Server:Storage:AddStorageBox", AddStorageBox);
			eventController.OnClient<int>("Server:Storage:Upgrade", Upgrade);
			eventController.OnClient<int>("Server:Storage:BuyStorage", BuyStorage);
            eventController.OnClient<int, ClPlayer?>("Server:Storage:SellStorage", SellStorage);
        }

		public async Task OnColshape(ClShape shape, IEntity entity, bool entered)
        {
            if (entity.Type != BaseObjectType.Player) return;

            var player = (ClPlayer)entity;
            if (player == null!) return;

            if (shape.ShapeType == ColshapeType.STORAGE_UPGRADE)
            {
                player.SetInteraction(Interactions.KEY_E, entered ? Interactions.E_STORAGE : Interactions.NONE);
            }

            if (shape.ShapeType == ColshapeType.JUMP_POINT && shape.Dimension == 0)
            {
                var storage = await _storageService.GetStorage(shape.Id);
                if (storage == null || storage.OwnerId != player.DbModel.Id) return;

                if (storage.OwnerId == -1)
                {
                    await player.Notify($"Lagerhalle ({storage.Id})", $"Die Lagerhalle hat kein Besitzer.", NotificationType.INFO);
                    return;
                }

                ClPlayer target = ClPlayer.All.FirstOrDefault(x => x.DbModel.Id == storage.OwnerId)!;
                if (target == null!) return;

                await player.Notify($"Lagerhalle ({storage.Id})", $"Besitzer: {target.DbModel.Name}", shape.Locked ? NotificationType.ERROR : NotificationType.SUCCESS);
            }
        }
        
        private async void OpenUpgrader(ClPlayer player, string eventKey)
		{
			if (player.DbModel == null || player.CurrentShape == null) return;

            var shape = player.CurrentShape;
            if(shape == null || shape.ShapeType != ColshapeType.STORAGE_UPGRADE) return;

            var storage = await _storageService.GetStorage(shape.Id);
            if(storage == null || storage.OwnerId != player.DbModel.Id) return;

            await player.ShowNativeMenu(true, new ClientNativeMenu($"Lagerhalle #{storage.Id}", new List<ClientNativeMenuItem>()
            {
                new ("Upgrade") { CallbackEvent = "Server:Storage:ShowUpgrades", CallbackArgs = new object[] { storage.Id }}
			}));
        }

		private async void ShowUpgrades(ClPlayer player, string eventKey, int id)
		{
			if (player.DbModel == null) return;

			var storage = await _storageService.GetStorage(id);
			if (storage == null || storage.OwnerId != player.DbModel.Id) return;
            var items = new List<ClientNativeMenuItem>();
            if (storage.Inventories.Count < storage.MaxInventorySlots()) items.Add(new ClientNativeMenuItem("Kisten ausbauen ($150000)") {CallbackEvent = "Server:Storage:AddStorageBox", CallbackArgs = new object[] {id}});
			if (storage.Inventories.Count == storage.MaxInventorySlots() && storage.WarehouseType < WarehouseType.HIGH_WAREHOUSE) items.Add(new ClientNativeMenuItem("Lagerhalle ausbauen ($150000)") { Close = true, CallbackEvent = "Server:Storage:Upgrade", CallbackArgs = new object[] {id}});

			await player.ShowNativeMenu(true, new ClientNativeMenu($"Lagerhalle #{id}", items));
		}

		private async void AddStorageBox(ClPlayer player, string eventKey, int id)
		{
			if (player.DbModel == null) return;

			if (player.DbModel.Money < 150000)
			{
                await player.Notify("Information", "Du hast nicht genug Geld um die Lagerhalle zu verbessern! ($150000)", NotificationType.ERROR);
                return;
			}

			var storage = await _storageService.GetStorage(id);
			if (storage == null || storage.OwnerId != player.DbModel.Id || player.Dimension != storage.Id) return;

			if (storage.MaxInventorySlots() <= storage.Inventories.Count)
            {
                await player.Notify("Information", "Du kannst keine weiteren Kisten ausbauen!", NotificationType.ERROR);
                return;
            }

			await player.RemoveMoney(150000);
			await _storageController.AddInventory(storage);
            await player.Notify("Information", $"Du hast erfolgreich eine Kiste ausgebaut!", NotificationType.SUCCESS);
		}

        private async void Upgrade(ClPlayer player, string eventKey, int id)
        {
			if (player.DbModel == null) return;

			if (player.DbModel.Money < 150000)
			{
				await player.Notify("Information", "Du hast nicht genug Geld um die Lagerhalle zu verbessern! ($150000)", NotificationType.ERROR);
				return;
			}

			var storage = await _storageService.GetStorage(id);
			if (storage == null || storage.OwnerId != player.DbModel.Id || player.Dimension != storage.Id) return;

            if (storage.WarehouseType == WarehouseType.HIGH_WAREHOUSE) return;

            await _storageController.Upgrade(storage);
			await player.SetPosition(storage.Position);
            await player.SetDimension(0);
			await player.Notify("Information", "Du hast dein Lagerhalle ausgebaut!", NotificationType.SUCCESS);
		}

		private async void BuyStorage(ClPlayer player, string eventKey, int storageId)
        {
            if (player == null || storageId > 0) return;

            var storageData = await _storageService.GetStorage(storageId);
            if (storageData == null) return;

            if (player.DbModel.BankMoney>= 80000 && storageData.OwnerId == 0)
            {
                _storageController.BuyStorage(player, storageId);

                await player.Notify($"Storage", $"Du hast die Lagerhalle {storageData.Id} für $80000 gekauft.", NotificationType.SUCCESS);
            }
        }

        private async void SellStorage(ClPlayer player, string eventKey, int storageId, ClPlayer? target = null)
        {
            if (player == null || storageId <= 0) return;

            var storageData = await _storageService.GetStorage(storageId);
            if (storageData == null || storageData.OwnerId != player.DbModel.Id) return;

            int newStoragePrice = 40000;

            switch (storageData.WarehouseType)
            {
                case WarehouseType.SMALL_WAREHOUSE: break;
                case WarehouseType.MEDIUM_WAREHOUSE:
                    newStoragePrice += 50000;
                    break;
                case WarehouseType.HIGH_WAREHOUSE:
                    newStoragePrice += 100000;
                    break;
                default: break;
            }

            var jumpPoint = await _jumpPointService.GetJumpPoint(storageData.Id + 300000);
            if (jumpPoint == null)
            {
                await player.Notify("Lagerhaus", "Ein Fehler ist aufgetreten, bitte versuche es erneut.", NotificationType.ERROR);
                return;
            }

            await player.SetPosition(jumpPoint.EntryPosition.Position);
            player.Rotation = jumpPoint.EntryPosition.Rotation;
            await player.SetDimension(jumpPoint.EntryPosition.Dimension);

            if (target == null)
            {
                int price = Convert.ToInt32(newStoragePrice * 0.75);
                await player.Notify($"Lagerhalle", $"Du hast deine Lagerhalle für ${price} verkauft.", NotificationType.SUCCESS);

                _storageController.SellStorage(player, storageId);
                return;
            }

            player.DbModel.Money += newStoragePrice;
            target.DbModel.Money -= newStoragePrice;

            await player.Notify($"Lagerhalle", $"Du hast deine Lagerhalle an {target.Name} für ${newStoragePrice} verkauft.", NotificationType.INFO);
            await player.Notify($"Lagerhalle", $"Du hast eine Lagerhalle von {player.Name} für ${newStoragePrice} abgekauft.", NotificationType.INFO);

            _storageController.SellStorage(player, storageId, target);
        }
	}
}
