using Backend.Services.Farming.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;

namespace Backend.Services.Farming
{
	public class FarmingService : IFarmingService
	{
		public List<FarmingModel> FarmingSpots { get; private set; } = new();

		public FarmingService()
		{
			using var ctx = new DBContext();

			FarmingSpots = new List<FarmingModel>(ctx.FarmingSpots);
		}

		public FarmingModel? GetFarmingSpot(int id)
		{
			return FarmingSpots.FirstOrDefault(x => x.Id == id);
		}

		public async Task AddFarmingSpot(FarmingModel model)
		{
			await using var ctx = new DBContext();

			FarmingSpots.Add(model);
			await ctx.FarmingSpots.AddAsync(model);
			await ctx.SaveChangesAsync();
		}

		public async Task UpdateFarmingSpot(FarmingModel model)
		{
			using var ctx = new DBContext();

			ctx.FarmingSpots.Update(model);
			await ctx.SaveChangesAsync();
		}
	}
}