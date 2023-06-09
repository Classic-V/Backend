using Backend.Utils.Models.Database;

namespace Backend.Services.VehicleShop.Interface
{
	public interface IVehicleShopService
	{
		List<VehicleShopModel> VehicleShops { get; }
		Task<VehicleShopModel?> GetVehicleShop(int id);
		Task AddVehicleShop(VehicleShopModel vehicleShop);
		Task UpdateVehicleShop(VehicleShopModel vehicleShop);
	}
}