using Backend.Controllers.Event.Interface;
using Backend.Controllers.Vehicle.Interface;
using Backend.Modules.Inventory;
using Backend.Services.Account.Interface;
using Backend.Services.Vehicles.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Inventory;
using Backend.Utils.Models.Player.Client;

namespace Backend.Modules.Admin.Menu;

public class GiveMenuModule : Module<GiveMenuModule>
{
    private IAccountService _accountService;
    private IVehicleService _vehicleService;
    
    public GiveMenuModule(IEventController eventController, IAccountService accountService, IVehicleService vehicleService) : base("GiveMenu")
    {
        _vehicleService = _vehicleService;
        _accountService = accountService;
        
        eventController.OnClient<int>("Server:Admin:OpenGiveMenu", OpenGiveMenu);
        eventController.OnClient<int>("Server:Admin:GiveItem:Open", OpenGiveItem);
        eventController.OnClient<int>("Server:Admin:GiveVehicle:Open", OpenGiveMenu);
        eventController.OnClient<int>("Server:Admin:GiveMoney:Open", OpenGiveMenu);
        eventController.OnClient<int, int[]>("Server:Admin:GiveItem:Give", GiveItems);
        eventController.OnClient<int>("Server:Admin:GiveMoney:Open", OpenGiveMoney);
        eventController.OnClient<int, int, bool>("Server:Admin:GiveMoney", GiveMoney);
        eventController.OnClient<int>("Server:Admin:GiveVehicle:Open", OpenGiveVehicle);
        eventController.OnClient<int, string, string>("Server:Admin:GiveVehicle", GiveVehicle);
    }

    private async void OpenGiveMenu(ClPlayer player, string eventKey, int accountId)
    {
        if (player.DbModel.AdminRank < AdminRank.ADMINISTRATOR) return;

        var items = new List<ClientNativeMenuItem>();
        
        items.Add(new ClientNativeMenuItem("Item geben") { Type = NativeMenuItemType.SUBMENU, CallbackEvent = "Server:Admin:GiveItem:Open", CallbackArgs = new object[] { accountId }});
        items.Add(new ClientNativeMenuItem("Fahrzeug geben") { Type = NativeMenuItemType.SUBMENU, CallbackEvent = "Server:Admin:GiveVehicle:Open", CallbackArgs = new object[] { accountId }});
        items.Add(new ClientNativeMenuItem("Geld geben") { Type = NativeMenuItemType.SUBMENU, CallbackEvent = "Server:Admin:GiveMoney:Open", CallbackArgs = new object[] { accountId }});

        await player.ShowNativeMenu(true, new ClientNativeMenu("Admin Menu", items, "Gegenstandsvergabe"));
    }

    #region Give Item

        private async void OpenGiveItem(ClPlayer player, string eventKey, int accountId)
        {
            if (player.DbModel.AdminRank < AdminRank.ADMINISTRATOR) return;

            var target = _accountService.GetAccount(accountId);
            if (target == null)
            {
                await player.Notify("Administration", "Der Account konnte nicht gefunden werden.", NotificationType.ERROR);
                return;
            }

            var items = new List<ClientNativeMenuItem>();
            foreach (var itemModel in InventoryModule.ItemModels)
            {
                items.Add(new ClientNativeMenuItem(itemModel.Name) { Type = NativeMenuItemType.INPUT, Data = "number", Value = 0});
            }
            
            items.Add(new ClientNativeMenuItem("Items geben") { Close = true, CallbackEvent = "Server:Admin:GiveItem:Give", CallbackArgs = new object[] { accountId }, IgnoreFilter = true});

            await player.ShowNativeMenu(true, new ClientNativeMenu("Admin Menu", items, "Item geben"));
        }

        private async void GiveItems(ClPlayer player, string eventKey, int accountId, params int[] itemCounts)
        {
            if (player.DbModel.AdminRank < AdminRank.ADMINISTRATOR) return;

            var target = _accountService.GetAccount(accountId);
            if (target == null)
            {
                await player.Notify("Administration", "Der Account konnte nicht gefunden werden.", NotificationType.ERROR);
                return;
            }
            
            int currentValue;
            ItemBase currentItem;
            for (int i = 0; i < itemCounts.Length; i++)
            {
                currentValue = itemCounts[i];
                currentItem = InventoryModule.ItemModels[i];
                
                if (currentValue <= 0) continue;

                target.Inventory.AddItem(currentItem, currentValue);
                await player.Notify("Administration",
                    "Du hast " + target.Name + " " + currentValue + "x " + currentItem.Name + " gegeben.", NotificationType.SUCCESS);
            }
        }

    #endregion
    
    #region Give Vehicle

    private async void OpenGiveVehicle(ClPlayer player, string eventKey, int accoundId)
    {
        if (player.DbModel.AdminRank < AdminRank.ADMINISTRATOR) return;

        var items = new List<ClientNativeMenuItem>();
        
        items.Add(new ClientNativeMenuItem("Model") {Type = NativeMenuItemType.INPUT});
        items.Add(new ClientNativeMenuItem("Kennzeichen") {Type = NativeMenuItemType.INPUT});
        items.Add(new ClientNativeMenuItem("Fahrzeug geben") { Close = true, CallbackEvent = "Server:Admin:GiveVehicle", CallbackArgs = new object[] {accoundId}});

        await player.ShowNativeMenu(true, new ClientNativeMenu("Admin Menu", items, "Geld geben"));
    }

    private async void GiveVehicle(ClPlayer player, string eventKey, int accountId, string model, string plate)
    {
        if (player.DbModel.AdminRank < AdminRank.ADMINISTRATOR) return;
        
        var target = _accountService.GetAccount(accountId);
        if (target == null)
        {
            await player.Notify("Administration", "Der Account konnte nicht gefunden werden.", NotificationType.ERROR);
            return;
        }

        var vehicleInfo = await _vehicleService.GetVehicleInfo(model);
        if (vehicleInfo == null)
        {
            await player.Notify("Administartion", "Für dieses Fahrzeug gibt es keine Informationen",
                NotificationType.ERROR);
            return;
        }
        
        var vehModel = new VehicleModel()
        {
            InfoModelId = vehicleInfo.Id,
            Owner = player.DbModel.Id,
            Garage = 28,
            Position = new PositionModel(),
            Trunk = new(vehicleInfo.TrunkWeight, vehicleInfo.TrunkSlots, InventoryType.VEHICLE_TRUNK),
            GloveBox = new(vehicleInfo.GloveBoxWeight, vehicleInfo.GloveBoxSlots, InventoryType.VEHICLE_GLOVEBOX),
            Type = VehicleType.PLAYER,
            Fuel = vehicleInfo.MaxFuel,
            Plate = plate,
            Parked = true,
        };

        await _vehicleService.AddVehicle(vehModel);
        await player.Notify("Administation", "Du hast dem Spieler ein Fahrzeug gegeben.", NotificationType.SUCCESS);
    }

    #endregion

    #region Give Money

    private async void OpenGiveMoney(ClPlayer player, string eventKey, int accoundId)
    {
        if (player.DbModel.AdminRank < AdminRank.ADMINISTRATOR) return;

        var items = new List<ClientNativeMenuItem>();
        
        items.Add(new ClientNativeMenuItem("Betrag") {Type = NativeMenuItemType.INPUT, Data = "number", Value = 0});
        items.Add(new ClientNativeMenuItem("Bank") { Type = NativeMenuItemType.CHECKBOX, Value = false});
        items.Add(new ClientNativeMenuItem("Geld geben") { Close = true, CallbackEvent = "Server:Admin:GiveMoney", CallbackArgs = new object[] {accoundId}});

        await player.ShowNativeMenu(true, new ClientNativeMenu("Admin Menu", items, "Geld geben"));
    }

    private async void GiveMoney(ClPlayer player, string eventKey, int accountId, int money, bool bank)
    {
        if (player.DbModel.AdminRank < AdminRank.ADMINISTRATOR) return;
        
        var target = _accountService.GetAccount(accountId);
        if (target == null)
        {
            await player.Notify("Administration", "Der Account konnte nicht gefunden werden.", NotificationType.ERROR);
            return;
        }

        if (target.IsLoggedIn)
        {
            var targetPlayer = ClPlayer.LoggedIn.Find(x => x.DbModel.Id == target.Id);
            if (targetPlayer == null) return;
            
            if (bank) targetPlayer.DbModel.BankMoney += money;
            else await targetPlayer.AddMoney(money);

            await player.Notify("Administration",
                "Du hast " + target.Name + " " + money + "$ " + (bank ? "auf die Bank" : "Bargeld") + " gegebe.", NotificationType.SUCCESS);
        }
        else
        {
            if (bank) target.BankMoney += money;
            else target.Money += money;
            
            await player.Notify("Administration",
                "Du hast " + target.Name + " " + money + "$ " + (bank ? "auf die Bank" : "Bargeld") + " gegebe.", NotificationType.SUCCESS);
        }
    }

    #endregion
}