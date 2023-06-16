using Backend.Utils.Models.Database;

namespace Backend.Services.Vehicles.Interface
{
    public interface IVehicleService
    {
        List<VehicleModel> Vehicles { get; }
        List<VehicleInfoModel> VehicleInfos { get; }
        Task AddVehicleInfo(VehicleInfoModel model);
        Task<VehicleInfoModel?> GetVehicleInfo(int id);
        Task<VehicleInfoModel?> GetVehicleInfo(string model);
		Task AddVehicle(VehicleModel vehicle);
        Task<VehicleModel?> GetVehicle(int id);
        Task DeleteVehicle(int id);
        Task UpdateVehicle(VehicleModel vehicle);
        Task SaveVehicels();

	}
}
