using Backend.Utils.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services.GasStation.Interface
{
	public interface IGasStationService
	{
		List<GasStationModel> GasStations { get; }
		Task<GasStationModel?> GetGasStation(int id);
		Task AddGasStation(GasStationModel gasStation);
	}
}