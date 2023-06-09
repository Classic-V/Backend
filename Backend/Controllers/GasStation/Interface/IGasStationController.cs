using Backend.Utils.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Controllers.GasStation.Interface
{
	public interface IGasStationController
	{
		void LoadGasStation(GasStationModel model);
	}
}