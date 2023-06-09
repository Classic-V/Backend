using AltV.Net.Elements.Entities;
using AltV.Net;
using Backend.Controllers.GasStation.Interface;
using Backend.Services.GasStation.Interface;
using Backend.Utils.Models.Database;

namespace Backend.Controllers.GasStation
{
	public class GasStationController : IGasStationController
	{
		private readonly IGasStationService _gasStationService;

		public GasStationController(IGasStationService gasStationService)
		{
			_gasStationService = gasStationService;

			_gasStationService.GasStations.ForEach(LoadGasStation);
		}

		public void LoadGasStation(GasStationModel model)
		{
			var blip = Alt.CreateBlip((byte)BlipType.Destination, model.Position);
			blip.Name = "Tankstelle";
			blip.Sprite = 361;
			blip.ShortRange = true;
			blip.Color = 64;
		}
	}
}