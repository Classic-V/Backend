using Backend.Controllers.Event.Interface;
using Backend.Services.GasStation.Interface;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Enums;

namespace Backend.Modules.GasStation
{
	public class GasStationModule : Module<GasStationModule>
	{
		private readonly IGasStationService _gasStationService;

		public GasStationModule(IEventController eventController, IGasStationService gasStationService) : base("GasStation")
		{
			_gasStationService = gasStationService;

			eventController.OnClient<int, int, int>("Server:GasStation:StartFueling", StartFueling);
		}

		private async void StartFueling(ClPlayer player, string eventKey, int vehicleId, int stationId, int amount)
		{
			if (player.DbModel == null) return;

			var station = await _gasStationService.GetGasStation(stationId);
			if (station == null) return;

			var vehicle = ClVehicle.All.FirstOrDefault(x => x.Id == vehicleId);
			if (vehicle == null || vehicle.DbModel == null) return;

			var price = station.Price * amount;

			if (vehicle.Engine)
			{
				await player.Notify("Tankstelle", "Der Motor muss ausgeschaltet sein!", NotificationType.ERROR);
				return;
			}

			if(vehicle.Position.Distance(station.Position) > 25)
			{
				await player.Notify("Tankstelle", "Das Fahrzeug hat sich zu weit entfernt!", NotificationType.ERROR);
				return;
			}

			if(price > player.DbModel.Money)
			{
				await player.Notify("Tankstelle", "Du hast nicht genug Geld dabei!", NotificationType.ERROR);
				return;
			}

			await player.StartInteraction(async () =>
			{
				if (vehicle == null || vehicle.Engine || vehicle.MaxFuel - vehicle.DbModel.Fuel < amount || vehicle.Position.Distance(player.Position) > 5 || vehicle.Position.Distance(station.Position) > 25) return;
				await player.RemoveMoney(price);
				await vehicle.SetFuel(vehicle.DbModel.Fuel + amount);
			}, amount * 300);
		}
	}
}