using Backend.Services.GasStation.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;

namespace Backend.Services.GasStation
{
	public class GasStationService : IGasStationService
	{
		public List<GasStationModel> GasStations { get; } = new();

		public GasStationService()
		{
			using var ctx = new DBContext();

			GasStations = new List<GasStationModel>(ctx.GasStations);
		}

		public Task<GasStationModel?> GetGasStation(int id)
		{
			return Task.FromResult(GasStations.FirstOrDefault(x => x.Id == id));
		}

		public async Task AddGasStation(GasStationModel gasStation)
		{
			await using var ctx = new DBContext();

			GasStations.Add(gasStation);
			ctx.GasStations.Add(gasStation);
			await ctx.SaveChangesAsync();
		}
	}
}