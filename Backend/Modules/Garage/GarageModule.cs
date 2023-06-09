using AltV.Net;
using AltV.Net.Async;
using Backend.Controllers.Garage.Interface;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Garage.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Utils.Enums;
using Backend.Utils.Models.Database;
using Backend.Controllers.Event.Interface;
using Backend.Services.Vehicles.Interface;

namespace Backend.Modules.Garage
{
	public class GarageModule : Module<GarageModule>
	{
		private readonly IGarageController _garageController;

		private readonly IVehicleService _vehicleService;

		public GarageModule(IEventController eventController, IGarageController garageController, IVehicleService vehicleService) : base("Garage")
		{
			_garageController = garageController;

			_vehicleService = vehicleService;

			eventController.OnClient<int>("Server:Garage:Open", Open);
			eventController.OnClient<int, int>("Server:Garage:TakeVehicle", TakeVehicle);
			eventController.OnClient<int, int>("Server:Garage:ParkVehicle", ParkVehicle);
		}

		private async void TakeVehicle(ClPlayer player, string eventKey, int garageId, int vehId)
		{
			await _garageController.TakeVehicle(player, garageId, vehId);
		}

		private async void ParkVehicle(ClPlayer player, string eventKey, int garageId, int vehId)
		{
			await _garageController.ParkVehicle(player, garageId, vehId);
		}

		private async void Open(ClPlayer player, string eventKey, int id)
		{
			var garage = await _garageController.GetGarageModel(id);
			if (garage == null || (garage.Type == GarageType.SWAT && (player.DbModel.Team < 1 || player.DbModel.Team > 2 || !player.DbModel.TeamStoragePermission)) || (garage.Type == GarageType.TEAM && player.DbModel.Team != garage.Owner)) return;

			var vehicles = new List<VehicleModel>();

			switch (garage.Type)
			{
				case GarageType.PUBLIC:
				case GarageType.PRIVATE:
					vehicles = await _garageController.GetPlayerVehicles(player, garage);
					break;
				case GarageType.TEAM:
					vehicles = await _garageController.GetTeamVehicles(player, garage);
					break;
				case GarageType.SWAT:
					vehicles = await _garageController.GetSwatVehicles(player, garage);
					break;
			}

			var data = new List<(VehicleModel, VehicleInfoModel)>();
			foreach (var vehicle in vehicles)
			{
				var model = await _vehicleService.GetVehicleInfo(vehicle.InfoModelId);
				if(model == null) continue;

				data.Add((vehicle, model));
			}

			await player.ShowComponent("Garage", true, JsonConvert.SerializeObject(new ClientGarageModel(id, data)));
		}
	}
}