using Backend.Controllers.Vehicle.Interface;
using Backend.Services.Vehicles.Interface;
using Backend.Utils.Models.Entities;

namespace Backend.Controllers.Vehicle
{
	public class VehicleController : IVehicleController
	{
		private readonly IVehicleService _vehicleService;

		public VehicleController(IVehicleService vehicleService)
		{
			_vehicleService = vehicleService;
		}

		public async Task SaveVehicle(ClVehicle vehicle)
		{
			if (vehicle.DbModel == null) return;

			await vehicle.Update();
			await _vehicleService.UpdateVehicle(vehicle.DbModel);
		}
	}
}