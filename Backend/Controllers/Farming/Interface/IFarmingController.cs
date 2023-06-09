using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Controllers.Farming.Interface
{
	public interface IFarmingController
	{
		void LoadFarmingSpot(FarmingModel model);
		void StartFarming(ClPlayer player, int spotId, int objectIndex);
		void OnTick();
	}
}