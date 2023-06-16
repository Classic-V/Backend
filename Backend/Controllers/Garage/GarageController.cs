using AltV.Net;
using AltV.Net.Async;
using Backend.Controllers.Garage.Interface;
using Backend.Data;
using Backend.Utils;
using Backend.Utils.Enums;
using Backend.Utils.Models.Database;
using AltV.Net.Elements.Entities;
using Backend.Utils.Models.Entities;
using AltV.Net.Data;
using Backend.Services.Garage.Interface;
using Backend.Services.Vehicles.Interface;
using Backend.Utils.Models;
using Backend.Utils.Streamer;
using Newtonsoft.Json;

namespace Backend.Controllers.Garage
{
	public class GarageController : IGarageController
	{
		private readonly IGarageService _garageService;
		private readonly IVehicleService _vehicleService;

		public GarageController(IGarageService garageService, IVehicleService vehicleService)
		{
			_garageService = garageService;
			_vehicleService = vehicleService;

			_garageService.Garages.ForEach(LoadGarage);
			_vehicleService.Vehicles.ForEach(LoadVehicle);
		}

		public Task<List<VehicleModel>> GetPlayerVehicles(ClPlayer player, GarageModel garage)
		{
			return Task.FromResult(_vehicleService.Vehicles.Where(x =>
			{
				if (ClVehicle.GetVehicle(x.Id) is null) return false;

				return x.Type == VehicleType.PLAYER &&
				        _vehicleService.GetVehicleInfo(x.InfoModelId).Result?.GarageType == garage.VehicleType &&
				        x.Owner == player.DbModel.Id &&
				        ((x.Parked && x.Garage == garage.Id) ||
				         player.Position.Distance(ClVehicle.GetVehicle(x.Id).Position) < 25);
			}).ToList());
		}

		public Task<List<VehicleModel>> GetTeamVehicles(ClPlayer player, GarageModel garage)
		{
			return Task.FromResult(_vehicleService.Vehicles.Where(x =>
			{
				if (ClVehicle.GetVehicle(x.Id) is null) return false;

				return x.Type == VehicleType.TEAM &&
				       _vehicleService.GetVehicleInfo(x.InfoModelId).Result?.GarageType == garage.VehicleType &&
				       x.Owner == player.DbModel.Team &&
				       ((x.Parked && x.Garage == garage.Id) || player.Position.Distance(ClVehicle.GetVehicle(x.Id).Position) < 25);
			}).ToList());
		}

		public Task<List<VehicleModel>> GetSwatVehicles(ClPlayer player, GarageModel garage)
		{
			return Task.FromResult(_vehicleService.Vehicles.Where(x =>
			{
				if (ClVehicle.GetVehicle(x.Id) is null) return false;

				return x.Type == VehicleType.SWAT &&
				       _vehicleService.GetVehicleInfo(x.InfoModelId).Result?.GarageType == garage.VehicleType &&
				       ((x.Parked && x.Garage == garage.Id) || player.Position.Distance(ClVehicle.GetVehicle(x.Id).Position) < 25);
			}).ToList());
		}

		public async Task<GarageModel?> GetGarageModel(int id)
		{
			return await _garageService.GetGarage(id);
		}

		public void LoadGarage(GarageModel garage)
		{
			var shape = (ClShape)Alt.CreateColShapeSphere(garage.Position, 2f);
			shape.Id = garage.Id;
			shape.ShapeType = ColshapeType.GARAGE;
			shape.Size = 2f;

			PedStreamer.AddObject(new Ped(0xE7565327, garage.PedPosition.X, garage.PedPosition.Y, garage.PedPosition.Z, garage.PedPosition.H, true, true, 0));

			if (garage.Type != GarageType.PUBLIC) return;

			var blip = Alt.CreateBlip((byte)BlipType.Destination, garage.Position);
			blip.Name = "Garage";
			blip.Sprite = 357;
			blip.ShortRange = true;
			blip.Color = 37;
		}

		private async void LoadVehicle(VehicleModel vehicle)
		{
			if (vehicle.Parked) return;

			var model = await _vehicleService.GetVehicleInfo(vehicle.InfoModelId);
			if (model == null) return;

			new ClVehicle(
				Alt.Core,
				model.Hash,
				vehicle.Position.Position,
				new Rotation(0, 0, (float)vehicle.Position.H),
				vehicle,
				model.MaxFuel
				);
		}

		public async Task TakeVehicle(ClPlayer player, int garageId, int vehicleId)
		{
			var garage = await _garageService.GetGarage(garageId);
			var vehicle = await _vehicleService.GetVehicle(vehicleId);
			if (garage == null || vehicle == null) return;

			var spawn = await GetFreeSpawn(garage);
			if (spawn == null) return;

			var model = await _vehicleService.GetVehicleInfo(vehicle.InfoModelId);
			if (model == null) return;

			new ClVehicle(Alt.Core, model.Hash, spawn.Position, new Rotation(0, 0, (float)spawn.H), vehicle, model.MaxFuel);
			vehicle.Parked = false;
			vehicle.Position = spawn;
		}

		public async Task ParkVehicle(ClPlayer player, int garageId, int vehicleId)
        {
            var vehicle = await _vehicleService.GetVehicle(vehicleId);
			if (vehicle == null) return;

			var veh = ClVehicle.All.FirstOrDefault(x => x.DbModel != null && x.DbModel.Id == vehicleId)!;
			vehicle.Parked = true;
			await veh.Update();
			ClVehicle.All.Remove(veh);
			veh.Remove();
		}

		private Task<PositionModel?> GetFreeSpawn(GarageModel garage)
		{
			return Task.FromResult(garage.Spawns.FirstOrDefault(x =>
			{
				bool free = true;
				for(var i = 0; i < ClVehicle.All.Count; i++)
				{
					if (ClVehicle.All[i].Position.Distance(x.Position) < 5)
					{
						free = false;
						break;
					}
				}

				return free;
			}));
		}
	}
}