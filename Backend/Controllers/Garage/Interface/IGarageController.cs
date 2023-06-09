using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;

namespace Backend.Controllers.Garage.Interface
{
	public interface IGarageController
	{
		void LoadGarage(GarageModel garage);

		Task<List<VehicleModel>> GetPlayerVehicles(ClPlayer player, GarageModel garage);
		Task<List<VehicleModel>> GetTeamVehicles(ClPlayer player, GarageModel garage);
		Task<List<VehicleModel>> GetSwatVehicles(ClPlayer player, GarageModel garage);
		Task<GarageModel?> GetGarageModel(int id);

		Task TakeVehicle(ClPlayer player, int garageId, int vehicleId);
		Task ParkVehicle(ClPlayer player, int garageId, int vehicleId);
	}
}