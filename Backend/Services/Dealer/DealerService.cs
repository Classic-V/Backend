using Backend.Services.Dealer.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;

namespace Backend.Services.Dealer
{
	public class DealerService : IDealerService
	{
		public List<DealerModel> Dealer { get; private set; } = new();

		public DealerService()
		{
			using var ctx = new DBContext();

			Dealer = new List<DealerModel>(ctx.Dealer);
		}

		public DealerModel? GetDealer(int id)
		{
			return Dealer.FirstOrDefault(x => x.Id == id);
		}

		public async Task AddDealer(DealerModel model)
		{
			await using var ctx = new DBContext();

			Dealer.Add(model);
			await ctx.Dealer.AddAsync(model);
			await ctx.SaveChangesAsync();
		}
	}
}