using Backend.Services.Crime.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;
using Newtonsoft.Json;

namespace Backend.Services.Crime
{
	public class CrimeService : ICrimeService
	{
		public List<CrimeModel> Crimes { get; } = new();
		public string CrimesJSON { get; private set; } = string.Empty;

        public CrimeService()
		{
			using var ctx = new DBContext();

			Crimes = new(ctx.Crimes);
			CrimesJSON = JsonConvert.SerializeObject(Crimes);
		}

        public Task<CrimeModel?> GetCrimeData(int id)
        {
            return Task.FromResult(Crimes.FirstOrDefault(x => x.Id == id));
        }

		public async Task AddCrime(CrimeModel model)
		{
			await using var ctx = new DBContext();

			Crimes.Add(model);
			await ctx.Crimes.AddAsync(model);
			await ctx.SaveChangesAsync();
		}
    }
}