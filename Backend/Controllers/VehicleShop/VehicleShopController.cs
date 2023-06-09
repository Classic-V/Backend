using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using Backend.Controllers.VehicleShop.Interface;
using Backend.Services.Vehicles.Interface;
using Backend.Services.VehicleShop.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.VehicleShop;
using Backend.Utils.Streamer;

namespace Backend.Controllers.VehicleShop
{
	public class VehicleShopController : IVehicleShopController
	{
		private readonly IVehicleShopService _vehicleShopService;
		private readonly IVehicleService _vehicleService;

		public VehicleShopController(IVehicleShopService vehicleShopService, IVehicleService vehicleService)
		{
			_vehicleShopService = vehicleShopService;
			_vehicleService = vehicleService;

			_vehicleShopService.VehicleShops.ForEach(LoadVehicleShop);
		}

		public void LoadVehicleShop(VehicleShopModel model)
		{
			var shape = (ClShape)Alt.CreateColShapeSphere(model.Position, 1.5f);
			shape.Id = model.Id;
			shape.ShapeType = ColshapeType.VEHICLE_SHOP;
			shape.Size = 1.5f;

			PedStreamer.AddObject(new Ped(0xE7565327, model.Ped.X, model.Ped.Y, model.Ped.Z, model.Ped.H, true, true, 0));

			var blip = Alt.CreateBlip(BlipType.Destination, model.Position);
			blip.Sprite = 225;
			blip.ShortRange = true;
			blip.Name = "Fahrzeughändler";
			blip.Color = 4;

			model.Vehicles.ForEach(LoadVehicleShopItem);
		}

		public async void LoadVehicleShopItem(VehicleShopItem model)
		{
			var info = await _vehicleService.GetVehicleInfo(model.VehicleInfoId);
			if (info == null) return;

			model.VehicleInfo = info;

			new ClVehicle(Alt.Core, info.Hash, model.Position.Position, model.Position.Rotation, null, 0)
			{
				Frozen = true,
				PrimaryColorRgb = new AltV.Net.Data.Rgba(0, 0, 0, 255),
				SecondaryColorRgb = new AltV.Net.Data.Rgba(0, 0, 0, 255),
				PearlColor = 0
			};
		}
	}
}