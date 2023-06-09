using Backend.Controllers.Database.Interface;
using Backend.Controllers.Player.Interface;
using Backend.Services.Gangwar.Interface;
using Backend.Services.Storage.Interface;
using Backend.Services.Team.Interface;
using Backend.Services.Vehicles.Interface;
using Backend.Utils.Models.Entities;

namespace Backend.Controllers.Database
{
	public class DatabaseController : IDatabaseController
	{
		private readonly IPlayerController _playerController;
		private readonly IStorageService _storageService;
		private readonly IVehicleService _vehicleService;
		private readonly ITeamService _teamService;
		private readonly IGangwarService _gangwarService;

		public DatabaseController(IPlayerController playerController, IStorageService storageService, IVehicleService vehicleService, ITeamService teamService, IGangwarService gangwarService)
		{
			_playerController = playerController;
			_storageService = storageService;
			_vehicleService = vehicleService;
			_teamService = teamService;
			_gangwarService = gangwarService;
		}

		public async Task SaveDatabase()
		{
			ClPlayer.All.ForEach(async x => await _playerController.SavePlayer(x));
			await _vehicleService.SaveVehicels();
			await _storageService.UpdateStorages();
			await _teamService.UpdateTeams();
			await _gangwarService.UpdateGangwars();
		}
	}
}