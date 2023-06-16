using AltV.Net;
using AltV.Net.Elements.Entities;
using Backend.Controllers.Event.Interface;
using Backend.Controllers.VehicleShop.Interface;
using Backend.Data;
using Backend.Services.Vehicles.Interface;
using Backend.Services.VehicleShop.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Player.Client;

namespace Backend.Modules.VehicleShop
{
	public class VehicleShopModule : Module<VehicleShopModule>, IEventColshape
	{
		private readonly IVehicleShopService _vehicleShopService;
		private readonly IVehicleService _vehicleService;

		public VehicleShopModule(IEventController eventController, IVehicleShopService vehicleShopService, IVehicleService vehicleService) : base("VehicleShop")
		{
			_vehicleShopService = vehicleShopService;
			_vehicleService = vehicleService;

			eventController.OnClient<int, int>("Server:VehicleShop:Buy", Buy);
			eventController.OnClient("Server:VehicleShop:Open", Open);
		}

		private async void Buy(ClPlayer player, string eventKey, int shopId, int vehIndex)
		{
			if (player.DbModel == null) return;

			var model = await _vehicleShopService.GetVehicleShop(shopId);
			if (model == null || player.Position.Distance(model.Position) > 2f) return;

			var vehicle = model.Vehicles[vehIndex];
			if (vehicle == null) return;

			var pos = model.Spawns.FirstOrDefault(x => ClVehicle.All.FirstOrDefault(e => e.Position.Distance(x.Position) < 2) == null);
			if (pos == null) return;

			if(player.DbModel.Money < vehicle.Price)
			{
				if(player.DbModel.BankMoney < vehicle.Price)
				{
					await player.Notify("Fahrzeughändler", "Du hast nicht genug Geld!", NotificationType.ERROR);
					return;
				}

				player.DbModel.BankMoney -= vehicle.Price;
			}
			else
			{
				await player.RemoveMoney(vehicle.Price);
			}

			var vehModel = new VehicleModel()
			{
				InfoModelId = vehicle.VehicleInfoId,
				Owner = player.DbModel.Id,
				Garage = 28,
				Position = pos,
				Trunk = new(vehicle.VehicleInfo.TrunkWeight, vehicle.VehicleInfo.TrunkSlots, InventoryType.VEHICLE_TRUNK),
				GloveBox = new(vehicle.VehicleInfo.GloveBoxWeight, vehicle.VehicleInfo.GloveBoxSlots, InventoryType.VEHICLE_GLOVEBOX),
				Type = VehicleType.PLAYER,
				Fuel = vehicle.VehicleInfo.MaxFuel
			};

			await _vehicleService.AddVehicle(vehModel);
			new ClVehicle(Alt.Core, vehicle.VehicleInfo.Hash, pos.Position, new(0, 0, pos.H), vehModel, vehicle.VehicleInfo.MaxFuel);
			await player.Notify("Fahrzeughändler", $"Du hast ein Fahrzeug für ${vehicle.Price} gekauft!", NotificationType.SUCCESS);
		}

		private async void Open(ClPlayer player, string eventKey)
		{
			if (player.DbModel == null || player.CurrentShape == null || player.CurrentShape.ShapeType != ColshapeType.VEHICLE_SHOP) return;

			var model = await _vehicleShopService.GetVehicleShop(player.CurrentShape.Id);
			if(model == null) return;

			var items = new List<ClientNativeMenuItem>();
			for (var i = 0; i < model.Vehicles.Count; i++)
				items.Add(new($"{model.Vehicles[i].VehicleInfo.Name} ${model.Vehicles[i].Price}")
				{
					Close = true, CallbackEvent = "Server:VehicleShop:Buy", CallbackArgs = new object[] { model.Id, i }
				});

			await player.ShowNativeMenu(true, new ClientNativeMenu("Fahrzeug Händler", items));
		}

		public Task OnColshape(ClShape shape, IEntity entity, bool entered)
		{
			if (entity.Type != BaseObjectType.Player || shape.ShapeType != ColshapeType.VEHICLE_SHOP) return Task.CompletedTask;

			var player = (ClPlayer)entity;
			player.SetInteraction(Interactions.KEY_E, entered ? "VEHICLE_SHOP" : Interactions.NONE);
			return Task.CompletedTask;
		}
	}
}