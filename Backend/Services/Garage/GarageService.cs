using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Services.Garage.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;

namespace Backend.Services.Garage
{
    public class GarageService : IGarageService
    {
        public List<GarageModel> Garages { get; private set; } = new();

        public GarageService()
        {
            using var ctx = new DBContext();

            Garages = new List<GarageModel>(ctx.Garages);
        }

        public async Task AddGarage(GarageModel garage)
        {
            await using var ctx = new DBContext();

            Garages.Add(garage);
			await ctx.Garages.AddAsync(garage);
            await ctx.SaveChangesAsync();
        }

        public Task<GarageModel?> GetGarage(int id)
        {
            return Task.FromResult(Garages.FirstOrDefault(x => x.Id == id));
        }

        public async Task UpdateGarage(GarageModel garage)
        {
            await using var ctx = new DBContext();

            ctx.Garages.Update(garage);
            await ctx.SaveChangesAsync();
        }
	}
}
