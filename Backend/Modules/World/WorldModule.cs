using Backend.Controllers.Database.Interface;
using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;

namespace Backend.Modules.World
{
	public class WorldModule : Module<WorldModule>, IEventIntervalTenMinute
	{
		public static int Weather = 0;
		private int _weatherTicks = 0;

		private readonly IDatabaseController _databaseController;

		public WorldModule(IDatabaseController databaseController) : base("World")
		{
			_databaseController = databaseController;
		}

		public Task OnEveryTenMinute()
		{
			_databaseController.SaveDatabase();

			if (_weatherTicks == 3)
			{
				Weather = new Random().Next(0, 9);
				ClPlayer.All.ForEach(x => x.Emit("Client:PlayerModule:SetWeather", Weather));
				_weatherTicks = -1;
			}

			_weatherTicks++;
			return Task.CompletedTask;
		}
	}
}