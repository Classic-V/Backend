using Backend.Utils.Models;
using Backend.Utils.Enums;
using Backend.Controllers.Event.Interface;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Services.ClothesShop.Interface;
using Backend.Services.ClothesShopItems.Interface;
using AltV.Net;
using Backend.Services.Ammunation.Interface;
using AltV.Net.Data;
using Backend.Utils.Models.Ammunation;
using Backend.Services.Crime.Interface;
using Backend.Services.Vehicles.Interface;
using System.Reflection.Emit;

namespace Backend.Modules.Chat.Public.Admin
{
    public class GamedesignCommandModule : CommandModule<GamedesignCommandModule>
    {
        private readonly IClothesShopService _clothesShopService;
        private readonly IClothesShopItemsService _clothesShopItemsService;
        private readonly IAmmunationService _ammunationService;
        private readonly ICrimeService _crimeService;
        private readonly IVehicleService _vehicleService;

        public GamedesignCommandModule(
            IEventController eventController, 
            IClothesShopService clothesShopService, 
            IClothesShopItemsService clothesShopItemsService,
            IAmmunationService ammunationService,
            ICrimeService crimeService,
			IVehicleService vehicleService) : base("GamedesignCommand", AdminRank.GAMEDESIGN)
        {
            _clothesShopService = clothesShopService;
            _clothesShopItemsService = clothesShopItemsService;
            _ammunationService = ammunationService;
            _crimeService = crimeService;
            _vehicleService = vehicleService;

			eventController.OnClient<string, string, int, int, int, int, int, int, int, int>("Server:Command:addcarbase", AddCarInfo);
			eventController.OnClient<int, int, int>("Server:Command:addteamcar", AddTeamCar);

			eventController.OnClient<int, int>("Server:Command:setmod", SetMod);

			eventController.OnClient<int, string, int, int>("Server:Command:addlaw", AddLaw);

			eventController.OnClient<int>("Server:Command:createclothesshop", CreateClothesShop);
            eventController.OnClient<string, int, int, int, bool, bool, int, int, string>("Server:Command:addclothingitem", AddClothingShopItem);
            eventController.OnClient<int, int, int, string, bool>("Server:Command:setclothing", SetClothing);

            eventController.OnClient<string>("Server:Command:createammunation", CreateAmmunation);
        }

		private async void AddCarInfo(ClPlayer player, string eventKey, string label, string name, int trunkW, int trunkS, int gloveW, int gloveS, int garageType, int maxFuel, int tax, int seats)
		{
			if (!CheckPermission(player)) return;

            await _vehicleService.AddVehicleInfo(new()
            {
                Name = label,
                Hash = Alt.Hash(name),
                TrunkWeight = trunkW,
                TrunkSlots = trunkS,
                GloveBoxWeight = gloveW,
                GloveBoxSlots = gloveS,
                MaxFuel = maxFuel,
                GarageType = (GarageVehicleType)garageType,
                Tax = tax,
                Seats = seats
            });

            await player.Notify("Gamedesign", $"Das Fahrzeug {label} wurde hinzugefügt!", NotificationType.SUCCESS);
		}

		private async void AddTeamCar(ClPlayer player, string eventKey, int team, int infoId, int garageId)
        {
            if (!CheckPermission(player)) return;

            var info = await _vehicleService.GetVehicleInfo(infoId);
            if(info == null) return;

            await _vehicleService.AddVehicle(new()
            {
                InfoModelId = info.Id,
                Owner = team,
                Parked = true,
                Garage = garageId,
                Trunk = new(info.TrunkWeight, info.TrunkSlots, InventoryType.VEHICLE_TRUNK),
				GloveBox = new(info.GloveBoxWeight, info.GloveBoxSlots, InventoryType.VEHICLE_GLOVEBOX),
                Type = VehicleType.TEAM,
                Fuel = info.MaxFuel
            });

			await player.Notify("Gamedesign", $"Das Fahrzeug {info.Name} wurde der Fraktion mit der ID {team} hinzugefügt!", NotificationType.SUCCESS);
		}

		private void SetMod(ClPlayer player, string eventKey, int mod, int val)
		{
			if (!CheckPermission(player) || !player.IsInVehicle) return;

            player.Vehicle.SetMod((byte)mod, (byte)val);
		}

		private async void AddLaw(ClPlayer player, string eventKey, int group, string name, int jailTime, int fine)
        {
            if (!CheckPermission(player)) return;

            var label = name.Replace('_', ' ');

            await _crimeService.AddCrime(new()
            {
                Group = group,
                Label = label,
                JailTime = jailTime,
                Fine = fine
            });

            await player.Notify("Gamedesign", $"Du hast {name} zu den Gesetzen hinzugefügt!", NotificationType.SUCCESS);
        }


		private async void CreateClothesShop(ClPlayer player, string eventKey, int type)
        {
            if (!CheckPermission(player)) return;

            var shop = new ClothesShopModel()
            {
                Position = new(player.Position, player.Rotation.Yaw),
                Type = (ClothesShopType)type
            };

            await _clothesShopService.AddClothesShop(shop);
            await player.Notify("GAMEDESIGN", $"Du hast ein Kleidershop hinzugefügt!", NotificationType.ERROR);
        }

        private async void AddClothingShopItem(ClPlayer player, string eventKey, string name, int componentId, int drawableId, int textureId, bool isProp, bool gender, int price, int type, string dlc = "")
		{
			if (!CheckPermission(player)) return;

			if (_clothesShopItemsService.ClothesShopItemList.FirstOrDefault(x => x.Component == componentId && x.Drawable == drawableId && x.Texture == textureId && x.IsProp == isProp && x.Gender == gender) != null)
			{
				await player.Notify("GAMEDESIGN", $"Diese Kleidung wurde bereits hinzugefügt!", NotificationType.ERROR);
				return;
            }

            if (name.Contains('_'))
            {
                name = name.Replace("_", " ");
            }

            ClothesShopItemModel itemModel = new ClothesShopItemModel(name, componentId, drawableId, textureId, dlc, price, isProp, gender, (ClothesShopItemType)type);

            await _clothesShopItemsService.AddClothesItem(itemModel);
            await player.Notify("GAMEDESIGN", $"Du hast ein Kleidershop-Item hinzugefügt!", NotificationType.SUCCESS);
        }

        private async void SetClothing(ClPlayer player, string eventKey, int componentId, int drawableId, int textureId, string dlc, bool isProp)
		{
			if (!CheckPermission(player)) return;

			if (dlc == "none") dlc = "";

            if (!isProp) await player.SetClothing(componentId, drawableId, textureId, Alt.Hash(dlc));
            else await player.SetProp(componentId, drawableId, textureId, Alt.Hash(dlc));
        }

        private async void CreateAmmunation(ClPlayer player, string eventKey, string name)
		{
			if (!CheckPermission(player)) return;

			if (name.Contains('_')) name.Replace('_', ' ');

            var model = new AmmunationModel(name, player.Position, new PositionModel(new Position(0, 0, 0), 0), new List<WeaponShopItemModel>());
            if (model == null) return;

            await _ammunationService.AddAmmunation(model);
            await player.Notify("GAMEDESIGN", "Du hast ein Ammunation erstellt.", NotificationType.INFO);
        }
    }
}
