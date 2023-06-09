using Backend.Services.VehicleShop.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;

namespace Backend.Services.VehicleShop
{
	public class VehicleShopService : IVehicleShopService
	{
		public List<VehicleShopModel> VehicleShops { get; } = new();

		public VehicleShopService()
		{
			using var ctx = new DBContext();

			VehicleShops = new List<VehicleShopModel>(ctx.VehicleShops);
		}

		public Task<VehicleShopModel?> GetVehicleShop(int id)
		{
			return Task.FromResult(VehicleShops.FirstOrDefault(x => x.Id == id));
		}

		public async Task AddVehicleShop(VehicleShopModel vehicleShop)
		{
			await using var ctx = new DBContext();

			VehicleShops.Add(vehicleShop);
			ctx.VehicleShops.Add(vehicleShop);
			await ctx.SaveChangesAsync();
		}

		public async Task UpdateVehicleShop(VehicleShopModel vehicleShop)
		{
			await using var ctx = new DBContext();

			ctx.VehicleShops.Update(vehicleShop);
			await ctx.SaveChangesAsync();
		}
	}
}