using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net.Elements.Entities;
using Backend.Services.Vehicles.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;

namespace Backend.Services.Vehicles
{
    public class VehicleService : IVehicleService
    {
        public List<VehicleModel> Vehicles { get; private set; }
        public List<VehicleInfoModel> VehicleInfos { get; private set; }

		public VehicleService()
        {
            using var ctx = new DBContext();

            Vehicles = new List<VehicleModel>(ctx.Vehicles);
            VehicleInfos = new List<VehicleInfoModel>(ctx.VehicleInfos);
        }

		public async Task AddVehicleInfo(VehicleInfoModel model)
		{
			await using var ctx = new DBContext();

			VehicleInfos.Add(model);
			await ctx.VehicleInfos.AddAsync(model);
			await ctx.SaveChangesAsync();
		}

        public Task<VehicleInfoModel?> GetVehicleInfo(int id)
        {
            return Task.FromResult(VehicleInfos.FirstOrDefault(x => x.Id == id));
        }

		public async Task AddVehicle(VehicleModel vehicle)
        {
            await using var ctx = new DBContext();

			Vehicles.Add(vehicle);
			await ctx.Vehicles.AddAsync(vehicle);
            await ctx.SaveChangesAsync();
        }

        public Task<VehicleModel?> GetVehicle(int id)
        {
            return Task.FromResult(Vehicles.FirstOrDefault(x => x.Id == id));
        }

        public async Task DeleteVehicle(int id)
        {
            var vehicleModel = await GetVehicle(id);
            if (vehicleModel == null) return;

            Vehicles.Remove(vehicleModel);

            await using var ctx = new DBContext();
            ctx.Vehicles.Remove(vehicleModel);
            await ctx.SaveChangesAsync();
        }

        public async Task UpdateVehicle(VehicleModel vehicle)
        {
            await using var ctx = new DBContext();
            ctx.Vehicles.Update(vehicle);
            await ctx.SaveChangesAsync();
        }

        public async Task SaveVehicels()
        {
            ClVehicle.All.ForEach(x => x.Update());

			await using var ctx = new DBContext();
			ctx.Vehicles.UpdateRange(Vehicles);
			await ctx.SaveChangesAsync();
		}
    }
}
