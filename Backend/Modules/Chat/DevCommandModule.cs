using Backend.Controllers.Event.Interface;
using Backend.Controllers.Garage.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Streamer;
using Newtonsoft.Json;
using Backend.Controllers.Team.Interface;
using Backend.Controllers.Player.Interface;
using Backend.Controllers.Storage.Interface;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Inventory;
using AltV.Net.Data;
using Backend.Services.Vehicles.Interface;
using AltV.Net;
using Backend.Controllers.Bank.Interface;
using Backend.Services.Shop.Interface;
using Backend.Controllers.Shop.Interface;
using Backend.Utils.Models.Shops;
using Backend.Modules.Inventory;
using Backend.Controllers.Processor.Interface;
using Backend.Utils;
using Backend.Services.Dealer.Interface;
using Backend.Services.Team.Interface;
using Backend.Utils.Models.Vehicle;
using Backend.Services.Drop.Interface;
using Backend.Services.Garage.Interface;
using Backend.Utils.Models.ClothesShop;
using Backend.Modules.World;
using Backend.Services.Farming.Interface;
using Backend.Controllers.Database.Interface;
using Backend.Modules.Laptop;
using Newtonsoft.Json.Linq;
using Backend.Services.GasStation.Interface;
using Backend.Controllers.GasStation.Interface;
using Backend.Controllers.House.Interface;
using Backend.Controllers.Wardrobe.Interface;
using Backend.Services.Animation.Interface;
using Backend.Services.Bank.Interface;
using Backend.Services.House.Interface;
using Backend.Services.TrainingStation.Interface;
using Backend.Services.Wardrobe.Interface;
using Backend.Utils.Models.Gangwar;
using Backend.Utils.Models.House;
using Backend.Services.ClothesShop.Interface;
using System.Numerics;

namespace Backend.Modules.Chat
{
	public class DevCommandModule : Module<DevCommandModule>
	{
		private readonly IPlayerController _playerController;
		private readonly IStorageController _storageController;
		private readonly IShopController _shopController;
		private readonly IProcessorController _processorController;
		private readonly ITeamController _teamController;
		private readonly IGarageController _garageController;
		private readonly IDatabaseController _databaseController;
		private readonly IGasStationController _gasStationController;
        private readonly IWardrobeController _wardrobeController;

        private readonly IVehicleService _vehicleService;
		private readonly IShopService _shopService;
		private readonly IDealerService _dealerService;
		private readonly ITeamService _teamService;
		private readonly IDropService _dropService;
		private readonly IGarageService _garageService;
		private readonly IFarmingService _farmingService;
		private readonly IGasStationService _gasStationService;
        private readonly IHouseService _houseService;
        private readonly IAnimationService _animationService;
        private readonly IBankService _bankService;
        private readonly ITrainingStationService _trainingStationService;
		private readonly IClothesShopService _clothesShopService;

        public DevCommandModule(
			IEventController eventController,
			IPlayerController playerController,
			IStorageController storageController,
			IShopController shopController,
			IProcessorController processorController,
			ITeamController teamController,
			IGarageController garageController,
			IDatabaseController databaseController,
			IGasStationController gasStationController,
            IWardrobeController wardrobeController,

            IVehicleService vehicleService,
			IShopService shopService,
			IDealerService dealerService,
			ITeamService teamService,
			IDropService dropService,
			IGarageService garageService,
			IFarmingService farmingService,
			IGasStationService gasStationService,
            IHouseService houseService,
            IAnimationService animationService,
            IBankService bankService,
            ITrainingStationService trainingStationService,
			IClothesShopService clothesShopService) : base("DevCommand")
		{
			_playerController = playerController;
			_storageController = storageController;
			_shopController = shopController;
			_processorController = processorController;
			_teamController = teamController;
			_garageController = garageController;
			_databaseController = databaseController;
			_gasStationController = gasStationController;
            _wardrobeController = wardrobeController;

			_vehicleService = vehicleService;
			_shopService = shopService;
			_dealerService = dealerService;
			_teamService = teamService;
			_dropService = dropService;
			_garageService = garageService;
			_farmingService = farmingService;
			_gasStationService = gasStationService;
			_houseService = houseService;
			_animationService = animationService;
			_bankService = bankService;
			_trainingStationService = trainingStationService;
			_clothesShopService = clothesShopService;

			

			

			eventController.OnClient<string>("Server:CommandFulltext:dispatch", DEBUG_Dispatch);

			eventController.OnClient<int>("Server:Command:addgaragespawn", AddGarageSpawn);
			eventController.OnClient<int>("Server:Command:addgarageped", AddGaragePed);
			eventController.OnClient<string, int, int, int>("Server:Command:creategarage", CreateGarage);

			eventController.OnClient<int, int>("Server:Command:adddropcrate", AddDropCrate);
			eventController.OnClient<string>("Server:Command:createdrop", CreateDrop);

			eventController.OnClient<int>("Server:Command:addteamlab", AddTeamLab);
			eventController.OnClient<string, string, int, int, int, int, int, string, int>("Server:Command:createteam", CreateTeam);

			eventController.OnClient("Server:Command:createdealer", CreateDealer);

			eventController.OnClient<int, int, int, int, int>("Server:Command:createprocessor", CreateProcessor);

			eventController.OnClient<int, int, int>("Server:Command:addshopitem", AddShopItem);
			eventController.OnClient<int>("Server:Command:addshopped", AddShopPed);
			eventController.OnClient<string, int, int>("Server:Command:createshop", CreateShop);

			eventController.OnClient("Server:Command:createstorage", CreateStorage);



			eventController.OnClient<int>("Server:Command:setlivery", SetLivery);
			eventController.OnClient<int, int>("Server:Command:setmod", SetMod);
			eventController.OnClient("Server:Command:senditems", SendItems);
			eventController.OnClient<int, int>("Server:Command:setvehicle", SetVehicle);
			eventController.OnClient<string, string, int>("Server:Command:anim", PlayAnimation);
			eventController.OnClient<float, float, float>("Server:Command:tpc", TeleportToCoord);
			eventController.OnClient("Server:Command:savedb", SaveDatabase);
			eventController.OnClient("Server:Command:getpos", GetPos);
			eventController.OnClient("Server:Command:getrot", GetRot);
			eventController.OnClient<uint>("Server:Command:createobj", CreateObject);
			eventController.OnClient<int>("Server:Command:removeobj", DestroyObject);
			eventController.OnClient<int>("Server:Command:setweather", SetWeather);

			eventController.OnClient<int>("Server:Command:addframingspot", AddPlant);

            eventController.OnClient<int, int>("Server:Command:addhouse", AddHouse);

            eventController.OnClient<string, int, string, string, int>("Server:Command:addanim", AddAnim);

            eventController.OnClient<string, int>("Server:Command:setbank", SetBank);

            eventController.OnClient<int>("Server:Command:addtrainingstation", AddTrainingStation);
        }

		private void SetLivery(ClPlayer player, string eventKey, int livery)
		{
			if (player.DbModel == null || !player.IsInVehicle || player.DbModel.AdminRank < AdminRank.PROJEKTLEITER) return;

			player.Vehicle.Livery = (byte)livery;
		}

		private void DEBUG_Dispatch(ClPlayer player, string eventKey, string message)
		{
			LaptopModule.Dispatches.Add(new(player.DbModel.Id, player.Name, message, player.Position, "00:00 01.01.0001", DispatchType.LSPD));
		}

		private void AddPlant(ClPlayer player, string eventKey, int id)
		{
			if (player.DbModel == null || player.DbModel.AdminRank < AdminRank.SUPPORTER) return;

			var model = _farmingService.GetFarmingSpot(id);
			if (model == null) return;

			model.Objects.Add(new Utils.Models.Farming.FarmingObjectModel()
			{
				Position = player.Position
			});

			_farmingService.UpdateFarmingSpot(model);
			ObjectStreamer.AddObject(model.ObjectHash, player.Position - new Position(0, 0, 1), new Rotation(), true, false, 0);
		}

		private void SetWeather(ClPlayer player, string eventKey, int type)
		{
			if (player.DbModel == null || player.DbModel.AdminRank < AdminRank.PROJEKTLEITER) return;

			WorldModule.Weather = type;
			ClPlayer.All.ForEach(x => x.Emit("Client:PlayerModule:SetWeather", type));
		}

		private void SetMod(ClPlayer player, string eventKey, int id, int value)
		{
			if (player.DbModel == null || !player.IsInVehicle || player.DbModel.AdminRank < AdminRank.PROJEKTLEITER) return;

			player.Vehicle.SetMod((byte)id, (byte)value);
		}

		private async void AddGarageSpawn(ClPlayer player, string eventKey, int id)
		{
			if (player.DbModel == null || player.DbModel.AdminRank < AdminRank.PROJEKTLEITER) return;

			if (!player.IsInVehicle)
			{
				await player.Notify("GAMEDESIGN", "Du musst in einem Fahrzeug sitzen!", NotificationType.ERROR);
				return;
			}

			var garage = await _garageService.GetGarage(id);
			if (garage == null)
			{
				await player.Notify("GAMEDESIGN", "Die Garage wurde nicht gefunden!", NotificationType.ERROR);
				return;
			}

			garage.Spawns.Add(new PositionModel(player.Vehicle.Position, player.Vehicle.Rotation.Yaw));
			await _garageService.UpdateGarage(garage);
			await player.Notify("GAMEDESIGN", "Der Spawn wurde hinzugefügt!", NotificationType.SUCCESS);
		}

		private async void AddGaragePed(ClPlayer player, string eventKey, int id)
		{
			if (player.DbModel == null || player.DbModel.AdminRank < AdminRank.PROJEKTLEITER) return;

			var garage = await _garageService.GetGarage(id);
			if(garage == null)
			{
				await player.Notify("GAMEDESIGN", "Die Garage wurde nicht gefunden!", NotificationType.ERROR);
				return;
			}

			garage.PedPosition = new PositionModel(player.Position - new Position(0, 0, 1), player.Rotation.Yaw);
			await _garageService.UpdateGarage(garage);
			_garageController.LoadGarage(garage);
			await player.Notify("GAMEDESIGN", "Erfolgreich hinzugefügt!", NotificationType.SUCCESS);
		}

		private async void CreateGarage(ClPlayer player, string eventKey, string name, int type, int owner, int vehType)
		{
			if (player.DbModel == null || player.DbModel.AdminRank < AdminRank.PROJEKTLEITER) return;

			var model = new GarageModel()
			{
				Name = name,
				Position = player.Position,
				Type = (GarageType)type,
				Owner = owner,
				VehicleType = (GarageVehicleType)vehType
			};

			await _garageService.AddGarage(model);
			await player.Copy(model.Id.ToString());
			await player.Notify("GAMEDESIGN", "Garage hinzugefügt!", NotificationType.SUCCESS);
		}

		private async void AddDropCrate(ClPlayer player, string eventKey, int dropId, int crate)
		{
			if (player.DbModel == null || player.DbModel.AdminRank < AdminRank.PROJEKTLEITER) return;

			var drop = _dropService.GetDrop(dropId);
			if (drop == null)
			{
				await player.Notify("GAMEDESIGN", "Der Drop konnte nicht gefunden werden!", NotificationType.ERROR);
				return;
			}

			switch (crate)
			{
				case 1:
					drop.LootCrate1 = new PositionModel(player.Position, player.Rotation.Yaw);
					break;
				case 2:
					drop.LootCrate2 = new PositionModel(player.Position, player.Rotation.Yaw);
					break;
				case 3:
					drop.LootCrate3 = new PositionModel(player.Position, player.Rotation.Yaw);
					break;
			}

			await _dropService.UpdateDrop(drop);
			await player.Notify("GAMEDESIGN", "Lootkiste hinzugefügt!", NotificationType.SUCCESS);
		}

		private async void CreateDrop(ClPlayer player, string eventKey, string location)
		{
			if (player.DbModel == null || player.DbModel.AdminRank < AdminRank.PROJEKTLEITER) return;

			var id = await _dropService.AddDrop(new DropModel()
			{
				Location = location.Replace('_',' '),
				Position = new PositionModel(player.Position, player.Rotation.Yaw)
			});

			await player.Copy(id.ToString());
			await player.Notify("GAMEDESIGN", "Die ID wurde in die zwischenablage kopiert!", NotificationType.SUCCESS);
		}

		private async void AddTeamLab(ClPlayer player, string eventKey, int teamId)
		{
			if (player.DbModel == null || player.DbModel.AdminRank < AdminRank.PROJEKTLEITER) return;

			var team = await _teamService.GetTeam(teamId);
			if(team == null)
			{
				await player.Notify("GAMEDESIGN", $"Das Team mit der ID {team} wurde nicht gefunden!", NotificationType.ERROR);
				return;
			}

			team.Laboratory = player.Position;
			await _teamService.UpdateTeam(team);

			_teamController.LoadTeam(team);
		}

		private async void CreateTeam(ClPlayer player, string eventKey, string name, string shortName, int r, int g, int b, int blipColor, int type, string meele, int meeleHash)
		{
			if (player.DbModel == null || player.DbModel.AdminRank < AdminRank.PROJEKTLEITER) return;

			var id = await _teamService.AddTeam(new TeamModel()
			{
				Name = name.Replace('_',' '),
				ShortName = shortName.Replace('_', ' '),
				Color = new RgbModel((byte)r, (byte)g, (byte)b, 255),
				BlipColor = (byte)blipColor,
				Type = (TeamType)type,
				MeeleWeapon = meele,
				MeeleWeaponHash = (uint)meeleHash,
				Drug = (type == 1 ? 301 : 302),
				ProcessedDrug = (type == 1 ? 401 : 402),
				Position = player.Position
			});

			await player.Copy(id.ToString());
			await player.Notify("GAMEDESIGN", "Die ID wurde in die zwischenablage kopiert!", NotificationType.SUCCESS);
		}

		private void CreateDealer(ClPlayer player, string eventKey)
		{
			if (player.DbModel == null || player.DbModel.AdminRank < AdminRank.PROJEKTLEITER) return;

			_dealerService.AddDealer(new DealerModel()
			{
				Position = new PositionModel(player.Position, player.Rotation.Yaw)
			});
		}

		

		

		private async void CreateProcessor(ClPlayer player, string eventKey, int neededItem, int neededItemAmount, int getItem, int getItemAmount, int duration)
		{
			if (player.DbModel == null || player.DbModel.AdminRank < AdminRank.PROJEKTLEITER) return;

			await _processorController.AddProcessor(new ProcessorModel()
			{
				NeededItem = neededItem,
				NeededItemAmount = neededItemAmount,
				GetItem = getItem,
				GetItemAmount = getItemAmount,
				Duration = duration
			});
		}

		private async void AddShopItem(ClPlayer player, string eventKey, int shopId, int itemId, int price)
		{
			if (player.DbModel == null || player.DbModel.AdminRank < AdminRank.PROJEKTLEITER) return;

			var shop = await _shopService.GetShop(shopId);
			if (shop == null) return;

			var item = InventoryModule.GetItemBase(itemId);
			if (item == null) return;

			shop.Items.Add(new ShopItemModel(item.Id, item.Name, price));
			await _shopController.SaveShops();
		}

		private async void AddShopPed(ClPlayer player, string eventKey, int shopId)
		{
			if (player.DbModel == null || player.DbModel.AdminRank < AdminRank.PROJEKTLEITER) return;

			var shop = await _shopService.GetShop(shopId);
			if(shop == null)
			{
				await player.Notify("GAMEDESIGN", "Der Shop konnte nicht gefunden werden.", NotificationType.ERROR);
				return;
			}

			shop.PedPosition = new PositionModel(player.Position - new Position(0,0,1), player.Rotation.Yaw);
			_shopController.LoadShop(shop);
			await _shopService.UpdateShop(shop);
		}

		private async void CreateShop(ClPlayer player, string eventKey, string name, int type, int owner)
		{
			if (player.DbModel == null || player.DbModel.AdminRank < AdminRank.PROJEKTLEITER) return;

			var id = await _shopController.CreateShop(new ShopModel()
			{
				Name = name.Replace('_', ' '),
				Position = player.Position,
				Type = (ShopType)type,
				Items = new List<ShopItemModel>(),
				OwnerId = owner
			});

			await player.Copy(id.ToString());
			await player.Notify("GAMEDESIGN", "Der Shop wurde erstellt! Die ID ist wurde in deine zwischenablage kopiert.", NotificationType.SUCCESS);
		}

		private async void CreateStorage(ClPlayer player, string eventKey)
		{
			if (player.DbModel == null || player.DbModel.AdminRank < AdminRank.GAMEDESIGN) return;

            await _storageController.CreateStorage(new StorageModel(
                -1,
                new Position(player.Position.X, player.Position.Y, player.Position.Z - 1),
                WarehouseType.SMALL_WAREHOUSE,
                new List<InventoryModel>()
            ));

            await player.Notify("GAMEDESIGN", "Du hast eine Lagerhalle erstellt.", NotificationType.INFO);
        }

		private void SetVehicle(ClPlayer player, string eventKey, int ownerId, int modelId)
		{
			if (player.DbModel == null || player.DbModel.AdminRank < AdminRank.PROJEKTLEITER) return;

			_vehicleService.AddVehicle(new VehicleModel()
			{
				Owner = ownerId,
				InfoModelId = modelId
			});
		}

		private async void SendItems(ClPlayer player, string eventKey)
		{
			if (player.DbModel == null || player.DbModel.AdminRank < AdminRank.PROJEKTLEITER) return;

			await DiscordWebhook.SendItems();
		}

		private void PlayAnimation(ClPlayer player, string eventKey, string dict, string name, int flag)
		{
			if (player.DbModel == null || player.DbModel.AdminRank < AdminRank.PROJEKTLEITER) return;

			player.Emit("Client:Animation:Play2", dict, name, flag);
		}

		private void TeleportToCoord(ClPlayer player, string eventKey, float x, float y, float z)
		{
			if (player.DbModel == null || player.DbModel.AdminRank < AdminRank.PROJEKTLEITER) return;

			player.SetPosition(new Position(x, y, z));
		}

		private async void SaveDatabase(ClPlayer player, string eventKey)
		{
			if (player.DbModel == null || player.DbModel.AdminRank < AdminRank.PROJEKTLEITER) return;

			await _databaseController.SaveDatabase();
		}

		private void GetPos(ClPlayer player, string eventKey)
		{
			if (player.DbModel == null || player.DbModel.AdminRank < AdminRank.PROJEKTLEITER) return;

			var pos = JsonConvert.SerializeObject(player.Position);

			player.Copy(pos);
			player.Notify("DEVELOPMENT", "Deine Position wurde in die zwischenablage kopiert!", NotificationType.SUCCESS);
			Console.WriteLine($"[DEBUG] {player.Name}: {pos}");
		}

		private void GetRot(ClPlayer player, string eventKey)
		{
			if (player.DbModel == null || player.DbModel.AdminRank < AdminRank.PROJEKTLEITER) return;

			var rot = JsonConvert.SerializeObject(player.Rotation);

			player.Copy(rot);
			player.Notify("DEVELOPMENT", "Deine Rotation wurde in die zwischenablage kopiert!", NotificationType.SUCCESS);
			Console.WriteLine($"[DEBUG] {player.Name}: {rot}");
		}

		private async void CreateObject(ClPlayer player, string eventKey, uint hash)
		{
			if (player.DbModel == null || player.DbModel.AdminRank < AdminRank.PROJEKTLEITER) return;

			var objId = ObjectStreamer.AddObject(hash, player.Position - new Position(0,0,1), player.Rotation, false, false, player.Dimension);
			await player.Copy(objId.ToString());
			Console.WriteLine($"[DEBUG] Object created. ID: {objId}");
		}

		private void DestroyObject(ClPlayer player, string eventKey, int id)
		{
			if (player.DbModel == null || player.DbModel.AdminRank < AdminRank.PROJEKTLEITER) return;

			ObjectStreamer.RemoveObject(id);
        }

        private async void AddHouse(ClPlayer player, string eventKey, int type, int price)
        {
            if (player.DbModel == null! || player.DbModel.AdminRank < AdminRank.GAMEDESIGN) return;

            var house = new HouseModel(0, (HouseType)type, player.Position, new List<RentMemberData>(),
                price, new InventoryModel(200f, 25, InventoryType.HOUSE_INVENTORY), 0);

            await _houseService.AddHouse(house);
            await player.Notify("GD", $"Haus ({house.Id})", NotificationType.INFO);

            var colShape = (ClShape)Alt.CreateColShapeSphere(house.Position, 0.75f);
            colShape.Id = house.Id;
            colShape.Size = 0.75f;
            colShape.ShapeType = ColshapeType.HOUSE;
            colShape.Dimension = 0;
            colShape.SetData("Entry", true);

            switch (house.Type)
            {
                case HouseType.LOW:
                    var lowShape = (ClShape)Alt.CreateColShapeSphere(HouseModel.LowPosition.Position, 0.75f);
                    lowShape.Id = house.Id;
                    lowShape.Size = 0.75f;
                    lowShape.ShapeType = ColshapeType.HOUSE;
                    lowShape.Dimension = house.Id + 50;
                    lowShape.SetData("Entry", false);
                    break;
                case HouseType.MEDIUM:
                    var mediumShape = (ClShape)Alt.CreateColShapeSphere(HouseModel.MediumPosition.Position, 0.75f);
                    mediumShape.Id = house.Id;
                    mediumShape.Size = 0.75f;
                    mediumShape.ShapeType = ColshapeType.HOUSE;
                    mediumShape.Dimension = house.Id + 50;
                    mediumShape.SetData("Entry", false);
                    break;
                case HouseType.HIGH:
                    var highShape = (ClShape)Alt.CreateColShapeSphere(HouseModel.HighPosition.Position, 0.75f);
                    highShape.Id = house.Id;
                    highShape.Size = 0.75f;
                    highShape.ShapeType = ColshapeType.HOUSE;
                    highShape.Dimension = house.Id + 50;
                    highShape.SetData("Entry", false);
                    break;
                default: break;
            }
        }

        private async void AddAnim(ClPlayer player, string eventKey, string name, int category, string animDict, string animName, int animFlag)
        {
            if (player.DbModel.AdminRank < AdminRank.GAMEDESIGN || player == null! || player.DbModel == null!) return;

            if (name.Contains('_'))
            {
                name = name.Replace("_", " ");
            }

            var data = new AnimationModel(name, (AnimationCategoryType)category, animDict, animName, animFlag);
            if (data == null!) return;

            await _animationService.AddAnimation(data);
            await player.Notify("GAMEDESIGN", $"Animation: {name} hinzugefügt!", NotificationType.INFO);
        }

        private async void SetBank(ClPlayer player, string eventKey, string name, int type)
        {
            if (player == null! || player.DbModel == null! || player.DbModel.AdminRank <= AdminRank.GAMEDESIGN) return;

            if (name.Contains("_"))
            {
                name = name.Replace("_", " ");
            }

            var bank = new BankModel(name, player.Position, (BankType)type);
			if (bank == null!) return;

            await _bankService.AddBank(bank);
            await player.Notify("GAMEDESIGN", $"Du hast eine Bank hinzugefügt!", NotificationType.INFO);
        }

        private async void AddTrainingStation(ClPlayer player, string eventKey, int type)
        {
            if (player == null! || player.DbModel == null! || player.DbModel.AdminRank <= AdminRank.GAMEDESIGN) return;

            var station = new TrainingStationModel(new PositionModel(player.Position, player.Rotation.Yaw),
                (TrainingType)type);
			if (station == null!) return;

            await _trainingStationService.AddTrainingStation(station);
            await player.Notify("GAMEDESIGN", $"Du hast eine Training Station hinzugefügt!", NotificationType.INFO);
        }
    }
}