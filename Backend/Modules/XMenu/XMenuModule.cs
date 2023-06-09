using AltV.Net;
using Backend.Controllers.Event.Interface;
using Backend.Controllers.Garage.Interface;
using Backend.Controllers.Vehicle.Interface;
using Backend.Modules.Inventory;
using Backend.Services.Garage.Interface;
using Backend.Services.GasStation.Interface;
using Backend.Services.Vehicles.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Input;
using Backend.Utils.Models.Inventory.Client;
using Newtonsoft.Json;

namespace Backend.Modules.XMenu
{
	public class XMenuModule : Module<XMenuModule>
	{
		private readonly IVehicleController _vehicleController;
		private readonly IGarageController _garageController;

		private readonly IGarageService _garageService;
		private readonly IVehicleService _vehicleService;
		private readonly IGasStationService _gasStationService;

		public XMenuModule(
			IEventController eventController,

			IVehicleController vehicleController,
			IGarageController garageController,
			
			IGarageService garageService,
			IVehicleService vehicleService,
			IGasStationService gasStationService) : base("XMenu")
		{
			_vehicleController = vehicleController;
			_garageController = garageController;

			_garageService = garageService;
			_vehicleService = vehicleService;
			_gasStationService = gasStationService;

			// Vehicle
			eventController.OnClient<int>("Server:XMenu:OpenFuel", Fuel);
			eventController.OnClient<int>("Server:XMenu:Eject", Eject);
			eventController.OnClient<int>("Server:XMenu:Park", Park);
			eventController.OnClient<int>("Server:XMenu:Repair", Repair);
			eventController.OnClient<int>("Server:XMenu:ToggleEngine", ToggleEngine);
			eventController.OnClient<int>("Server:XMenu:LockVehicle", LockVehicle);
			eventController.OnClient<int>("Server:XMenu:LockTrunk", LockTrunk);

			// Player
			eventController.OnClient<int>("Server:XMenu:Revive", RevivePlayer);
			eventController.OnClient<int>("Server:XMenu:Search", SearchPlayer);
			eventController.OnClient<int, bool>("Server:XMenu:Cuff", CuffPlayer);
			eventController.OnClient<int, bool>("Server:XMenu:Rope", RopePlayer);
			eventController.OnClient<int>("Server:XMenu:Grab", GrabIntoVehicle);
			eventController.OnClient<int>("Server:XMenu:GiveLicense", GivePlayerLicenses);
			eventController.OnClient<int>("Server:XMenu:TakeLicense", TakePlayerLicenses);
			eventController.OnClient<int>("Server:XMenu:GiveId", GivePlayerIdCard);
			eventController.OnClient<int>("Server:XMenu:TakeId", TakePlayerIdCard);
			eventController.OnClient<int>("Server:XMenu:GiveMoney", GivePlayerMoney);
			eventController.OnClient<int>("Server:XMenu:Stabilize", StabilizePlayer);
		}

		#region Vehicle
		private async void Fuel(ClPlayer player, string eventKey, int vehId)
		{
			if (player.DbModel == null) return;

			var veh = ClVehicle.All.FirstOrDefault(x => x.Id == vehId);
			if (veh == null || veh.DbModel == null || player.Position.Distance(veh.Position) > 5) return;

			var station = _gasStationService.GasStations.FirstOrDefault(x => x.Position.Distance(veh.Position) <= 25);
			if(station == null)
			{
				await player.Notify("Tankstelle", "Du musst bei einer Tankstelle sein!", NotificationType.ERROR);
				return;
			}

			if (veh.Engine)
			{
				await player.Notify("Tankstelle", "Der Motor muss ausgeschaltet sein!", NotificationType.ERROR);
				return;
			}

			var data = new
			{
				Id = station.Id,
				Vehicle = vehId,
				Max = veh.MaxFuel - veh.DbModel.Fuel,
				Price = station.Price
			};

			await player.ShowComponent("GasStation", true, JsonConvert.SerializeObject(data));
		}

		private async void Eject(ClPlayer player, string eventKey, int vehId)
		{
			if (player.DbModel == null || !player.IsInVehicle || player.Vehicle.Id != vehId) return;
			var veh = (ClVehicle)player.Vehicle;
			if (veh == null || veh.DbModel == null) return;

			var info = await _vehicleService.GetVehicleInfo(veh.DbModel.InfoModelId);
			if (info == null) return;

			var players = ClPlayer.All.Where(x => x.Vehicle == veh);
			var data = new List<bool>();

			for(var i = 0; i < info.Seats; i++)
				data.Add(players.FirstOrDefault(x => x.Seat == i+1) != null);

			await player.ShowComponent("Eject", true, JsonConvert.SerializeObject(data));
		}

		private async void Park(ClPlayer player, string eventKey, int vehId)
		{
			if (player.DbModel == null) return;

			var veh = ClVehicle.All.FirstOrDefault(x => x.Id == vehId);
			if (veh == null || veh.DbModel == null || player.Position.Distance(veh.Position) > 5 || (veh.DbModel.Type == VehicleType.PLAYER && veh.DbModel.Owner != player.DbModel.Id || veh.DbModel.Type == VehicleType.TEAM && veh.DbModel.Owner != player.DbModel.Team)) return;

			var garage = _garageService.Garages.FirstOrDefault(x => CheckVehiclePark(player, veh, x));
			if(garage == null)
			{
				await player.Notify("GARAGE", "Es ist keine Garage in der Nähe!", NotificationType.ERROR);
				return;
			}

			await _garageController.ParkVehicle(player, garage.Id, veh.DbModel.Id);
			await player.Notify("GARAGE", "Fahrzeug eingeparkt.", NotificationType.SUCCESS);
		}

		private bool CheckVehiclePark(ClPlayer player, ClVehicle vehicle, GarageModel garage)
		{
			if (vehicle.DbModel == null) return false;

			return garage.Position.Distance(vehicle.Position) < 25 &&
				(vehicle.DbModel.Type == VehicleType.TEAM && garage.Type == GarageType.TEAM ||
				(vehicle.DbModel.Type == VehicleType.PLAYER && garage.Type == GarageType.PUBLIC || garage.Type == GarageType.PRIVATE)) &&
				vehicle.DbModel.Type == VehicleType.SWAT && garage.Type == GarageType.SWAT &&

				(_vehicleService.GetVehicleInfo(vehicle.DbModel.InfoModelId).Result?.GarageType == garage.VehicleType) &&
				(garage.Type == GarageType.TEAM && vehicle.DbModel.Owner == garage.Owner) ||
				(garage.Type == GarageType.SWAT && player.DbModel.Team > 0 && player.DbModel.Team < 3 && player.DbModel.TeamStoragePermission);
		}

		private async void Repair(ClPlayer player, string eventKey, int vehId)
		{
			if (player.DbModel == null) return;

			var veh = ClVehicle.All.FirstOrDefault(x => x.Id == vehId);
			if (veh == null || player.Position.Distance(veh.Position) > 5) return;

			if(veh.Engine)
			{
				await player.Notify("FAHRZEUG", "Der Motor muss ausgeschaltet sein.", NotificationType.ERROR);
				return;
			}

			var repairItem = InventoryModule.GetItemBase(11)!;

			if (!player.DbModel.Inventory.HasItems(repairItem, 1)) return;

			player.PlayAnimation(AnimationType.REPAIR);
			await player.StartInteraction(() =>
			{
				if (veh == null || player.Position.Distance(veh.Position) > 5 || !player.DbModel.Inventory.HasItems(repairItem, 1)) return;
				veh.Repair();
			}, 4500);
		}

		private async void ToggleEngine(ClPlayer player, string eventKey, int vehId)
		{
			if (player.DbModel == null) return;

			var veh = ClVehicle.All.FirstOrDefault(x => x.Id == vehId);
			if (veh == null || player.Seat != 1 || player.Position.Distance(veh.Position) > 5 || (veh.DbModel != null && (veh.DbModel.Type == VehicleType.PLAYER && veh.DbModel.Owner != player.DbModel.Id || veh.DbModel.Type == VehicleType.TEAM && veh.DbModel.Owner != player.DbModel.Team || veh.DbModel.Type == VehicleType.SWAT && (player.DbModel.Team < 1 || player.DbModel.Team > 2 || !player.DbModel.TeamStoragePermission)))) return;

			await veh.SetEngineState(!veh.Engine);
			if (veh.Engine) await player.Notify("FAHRZEUG", $"Motor angeschaltet.", NotificationType.SUCCESS);
			else await player.Notify("FAHRZEUG", $"Motor ausgeschaltet.", NotificationType.ERROR);
		}

		private async void LockVehicle(ClPlayer player, string eventKey, int vehId)
		{
			if (player.DbModel == null) return;

			var veh = ClVehicle.All.FirstOrDefault(x => x.Id == vehId);
			if (veh == null || player.Position.Distance(veh.Position) > 5 || (veh.DbModel != null && (veh.DbModel.Type == VehicleType.PLAYER && veh.DbModel.Owner != player.DbModel.Id || veh.DbModel.Type == VehicleType.TEAM && veh.DbModel.Owner != player.DbModel.Team || veh.DbModel.Type == VehicleType.SWAT && (player.DbModel.Team < 1 || player.DbModel.Team > 2 || !player.DbModel.TeamStoragePermission) || veh.DbModel.Type == VehicleType.TEMPORARY && veh.DbModel.Owner != player.DbModel.Id))) return;

			await veh.SetLockState(!veh.Locked);
			if (veh.Locked) await player.Notify("FAHRZEUG", $"Fahrzeug abgeschlossen.", NotificationType.ERROR);
			else await player.Notify("FAHRZEUG", $"Fahrzeug aufgeschlossen.", NotificationType.SUCCESS);
		}

		private async void LockTrunk(ClPlayer player, string eventKey, int vehId)
		{
			if (player.DbModel == null) return;

			var veh = ClVehicle.All.FirstOrDefault(x => x.Id == vehId);
			if (veh == null || veh.Locked || player.Position.Distance(veh.Position) > 5) return;

			veh.TrunkLocked = !veh.TrunkLocked;
			veh.SetDoorState(5, (byte)(veh.TrunkLocked ? 0 : 7));
			if (veh.TrunkLocked) await player.Notify("FAHRZEUG", $"Kofferraum abgeschlossen.", NotificationType.ERROR);
			else await player.Notify("FAHRZEUG", $"Kofferraum aufgeschlossen.", NotificationType.SUCCESS);
		}
		#endregion

		#region Player

		private async void RevivePlayer(ClPlayer player, string eventKey, int playerId)
		{
			if (!player.DbModel.Duty)
			{
				await player.Notify("Medic System", "Du musst im Dienst sein um jemanden zu behandeln!", NotificationType.ERROR);
				return;
			}

			var target = ClPlayer.All.FirstOrDefault(x => x.Id == playerId);
			if (target == null || target.DbModel == null || target.DbModel.Alive || !target.InHostpital) return;

			if (!target.DbModel.Stabilized)
			{
				await player.Notify("Information", "Die Person muss Stabilisiert sein.", NotificationType.INFO);
				return;
			}

			if (target.DbModel.IsKoma)
			{
				await player.Notify("Information", "Diese Person scheint bereits im Koma zu sein.", NotificationType.INFO);
				return;
			}

			player.PlayAnimation(AnimationType.USE_MEDIKIT);
			await player.StartInteraction(async () =>
			{
				if (target == null) return;

				target.DbModel.Alive = true;
				target.DbModel.Injury = 0;
				target.DbModel.IsKoma = false;
				await target.SetStabilized(false);
				target.Emit("Client:PlayerModule:SetAlive", true);
				target.StopAnimation();
				await target.SetHealth(200, 0);
				await target.SetGodmode(false);
				target.SetStreamSyncedMetaData("ALIVE", true);
				await player.Notify("Information", "Du hast jemanden medizinisch behandelt!", NotificationType.INFO);
			}, 30000);
		}

		private async void SearchPlayer(ClPlayer player, string eventKey, int playerId)
		{
			if (player.DbModel == null) return;

			var target = ClPlayer.All.FirstOrDefault(x => x.Id == playerId);
			if (target == null || target.DbModel == null || (target.DbModel.Alive && !target.DbModel.Roped && !target.DbModel.Cuffed)) return;

			var weapons = new List<InventoryWeaponDataModel>();
			target.DbModel.Loadout.ForEach(x => weapons.Add(new InventoryWeaponDataModel(x.Hash, x.Ammo)));
			var data = JsonConvert.SerializeObject(new InventoryDataModel(player.DbModel.Inventory, target.DbModel.Id, target.DbModel.Inventory, -1, weapons));

			if (target.HasBeenSearched)
			{
				await player.ShowComponent("Inventory", true, data);
			}
			else
			{
				player.PlayAnimation(AnimationType.SEARCH);
				await player.StartInteraction(async () =>
				{
					if (target == null || target.DbModel == null) return;

					target.HasBeenSearched = true;

					await player.ShowComponent("Inventory", true, data);
				}, 10000);
			}
		}

		private async void CuffPlayer(ClPlayer player, string eventKey, int playerId, bool state)
		{
			if (player.DbModel == null) return;

			var target = ClPlayer.All.FirstOrDefault(x => x.Id == playerId);
			if (target == null || target.DbModel == null || !target.DbModel.Alive || target.DbModel.Roped || target.DbModel.Cuffed == state) return;

			if (state)
			{
				if (!player.DbModel.Inventory.RemoveItem(InventoryModule.GetItemBase(10)!, 1)) return;

				await target.SetCuffed(true);
				target.PlayAnimation(AnimationType.CUFFED);
				await target.StopInteraction();
			}
			else
			{
				if (player.DbModel.Team != 1 && player.DbModel.Team != 2) return;

				await target.SetCuffed(false);
				target.StopAnimation();
			}
		}

		private async void RopePlayer(ClPlayer player, string eventKey, int playerId, bool state)
		{
			if (player.DbModel == null) return;

			var target = ClPlayer.All.FirstOrDefault(x => x.Id == playerId);
			if (target == null || target.DbModel == null || !target.DbModel.Alive || target.DbModel.Cuffed || target.DbModel.Roped == state) return;

			if(state)
			{
				if (!player.DbModel.Inventory.RemoveItem(InventoryModule.GetItemBase(9)!, 1)) return;

				await target.SetRoped(true);
				target.PlayAnimation(AnimationType.ROPED);
				await target.StopInteraction();
			}
			else
			{
				await target.SetRoped(false);
				target.StopAnimation();
			}
		}

		private async void GrabIntoVehicle(ClPlayer player, string eventKey, int playerId)
		{
			if (player.DbModel == null) return;

			var target = ClPlayer.All.FirstOrDefault(x => x.Id == playerId);
			if (target == null || target.DbModel == null || (target.DbModel.Alive && !target.DbModel.Roped && !target.DbModel.Cuffed)) return;

			ClVehicle? vehicle = null;
			float dist = 5;
			foreach(var veh in ClVehicle.All)
			{
				var distance = target.Position.Distance(veh.Position);
				if (distance < dist)
				{
					vehicle = veh;
					dist = distance;
				}
			}

			if(vehicle == null)
			{
				await player.Notify("FEHLER", "Es ist kein Fahrzeug in der Nähe!", NotificationType.ERROR);
				return;
			}

			player.PlayAnimation(AnimationType.DRAG_IN_VEHICLE);
			await player.StartInteraction(() =>
			{
				if (target == null || !target.Exists || target.DbModel == null || (!target.DbModel.Alive && !target.DbModel.Roped && !target.DbModel.Cuffed) || vehicle.Position.Distance(target.Position) > 5) return;
				target.SetIntoVehicle(vehicle, 4);
			}, 7500);
		}

		private void TakePlayerLicenses(ClPlayer player, string eventKey, int playerId)
		{
			if (player.DbModel == null) return;

			var target = ClPlayer.All.FirstOrDefault(x => x.Id == playerId);
			if (target == null || target.DbModel == null || (target.DbModel.Alive && !target.DbModel.Roped && !target.DbModel.Cuffed)) return;

			// Todo: add data
			player.Emit("Client:Hud:ShowLicense", "");
		}

		private void GivePlayerLicenses(ClPlayer player, string eventKey, int playerId)
		{
			if (player.DbModel == null) return;

			var target = ClPlayer.All.FirstOrDefault(x => x.Id == playerId);
			if (target == null || target.DbModel == null) return;

			// Todo: add data
			target.Emit("Client:Hud:ShowLicense", "");
		}

		private void TakePlayerIdCard(ClPlayer player, string eventKey, int playerId)
		{
			if (player.DbModel == null) return;

			var target = ClPlayer.All.FirstOrDefault(x => x.Id == playerId);
			if (target == null || target.DbModel == null || (target.DbModel.Alive && !target.DbModel.Roped && !target.DbModel.Cuffed)) return;

			// Todo: add data
			player.Emit("Client:Hud:ShowIdCard", "");
		}

		private void GivePlayerIdCard(ClPlayer player, string eventKey, int playerId)
		{
			if (player.DbModel == null) return;

			var target = ClPlayer.All.FirstOrDefault(x => x.Id == playerId);
			if (target == null || target.DbModel == null) return;

			// Todo: add data
			target.Emit("Client:Hud:ShowIdCard", "");
		}

		private async void GivePlayerMoney(ClPlayer player, string eventKey, int playerId)
		{
			if (player.DbModel == null) return;

			var target = ClPlayer.All.FirstOrDefault(x => x.Id == playerId);
			if (target == null || target.DbModel == null) return;

			await player.ShowComponent("Input", true, JsonConvert.SerializeObject(new InputDataModel(
				"Geld übergabe",
				"Wie viel Geld möchtest du dieser Person übergeben?",
				InputType.TEXT,
				"Server:Player:GiveMoney",
				playerId
				)));
		}

		private async void StabilizePlayer(ClPlayer player, string eventKey, int playerId)
		{
			var target = ClPlayer.All.FirstOrDefault(x => x.Id == playerId);
			if (target == null || target.DbModel == null || target.DbModel.Alive) return;

			if (target.DbModel.IsKoma)
			{
				await player.Notify("Information", "Diese Person scheint bereits im Koma zu sein.", NotificationType.INFO);
				return;
			}

			if (target.DbModel.Stabilized)
			{
				await player.Notify("INFORMATION", GetInjuryMessage(target.DbModel.Injury), NotificationType.INFO);
				return;
			}

			var medikit = InventoryModule.GetItemBase(1)!;

			if(!player.DbModel.Inventory.HasItems(medikit, 1))
			{
				await player.Notify("Erste Hilfe", "Du benötigst einen Verbandskasten!", NotificationType.INFO);
				return;
			}

			player.PlayAnimation(AnimationType.USE_MEDIKIT);
			await player.StartInteraction(async () =>
			{
				if (target == null || !target.Exists || target.DbModel.Alive) return;

				if(target.DbModel.IsKoma)
				{
					await player.Notify("INFORMATION", "Die Person ist bereits im Koma!", NotificationType.ERROR);
					return;
				}

				if((target.DbModel.Injury == InjuryType.FALL_DAMAGE || target.DbModel.Injury == InjuryType.PUNCH) && new Random().Next(0, 100) < 50)
				{
					target.DbModel.Alive = true;
					target.DbModel.Injury = 0;
					target.DbModel.IsKoma = false;
					target.Emit("Client:PlayerModule:SetAlive", true);
					target.StopAnimation();
					await target.SetHealth(200, 0);
					await target.SetGodmode(false);
					target.SetStreamSyncedMetaData("ALIVE", true);

					await player.Notify("ERSTE HILFE", "Du konntest der Person durch deine Behandlung aufhelfen!", NotificationType.INFO);
					await target.Notify("ERSTE HILFE", "Dir geht es durch die Behandlung schon viel besser!", NotificationType.INFO);
					return;
				}

				await target.SetStabilized(true);
				await player.Notify("INFORMATION", "Du hast die Person stabilisiert!", NotificationType.SUCCESS);
			}, 20000);
		}

		private string GetInjuryMessage(InjuryType type)
		{
			var result = "";

			switch (type)
			{
				case InjuryType.FALL_DAMAGE:
					result = "Die Person scheint aus größerer höhe gefallen zu sein!";
					break;
				case InjuryType.PUNCH:
					result = "Die Person hat auffällig viele blaue Flecke!";
					break;
				case InjuryType.DROWN:
					result = "Die Person scheint nicht mehr zu Atmen!";
					break;
				case InjuryType.SLICE:
					result = "Die Person hat einige Schnittwunden!";
					break;
				case InjuryType.SHOT_LOW:
					result = "Die Person scheint angeschossen worden zu sein!";
					break;
				case InjuryType.VEHICLE:
					result = "Die Person scheint angefahren worden zu sein!";
					break;
				case InjuryType.SHOT_HIGH:
					result = "Die Person scheint durch ein großes Kaliber angeschossen worden zu sein!";
					break;
				case InjuryType.FIRE:
					result = "Die Person weist einige Verbrennungen auf!";
					break;
				case InjuryType.EXPLOSION:
					result = "Die Person scheint von mehreren Schrapnellen getroffen worden zu sein!";
					break;
			}

			return result;
		}
		#endregion
	}
}