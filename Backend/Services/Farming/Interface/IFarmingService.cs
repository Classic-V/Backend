using Backend.Utils.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services.Farming.Interface
{
	public interface IFarmingService
	{
		List<FarmingModel> FarmingSpots { get; }
		FarmingModel? GetFarmingSpot(int id);
		Task AddFarmingSpot(FarmingModel model);
		Task UpdateFarmingSpot(FarmingModel model);
	}
}