using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Controllers.Processor.Interface
{
	public interface IProcessorController
	{
		void LoadProcessor(ProcessorModel model);
		Task ProcessInventory(ClPlayer player, int id);
		Task ProcessVehicle(ClPlayer player, int id, int vehId);
		Task AddProcessor(ProcessorModel model);
	}
}