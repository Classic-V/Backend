using Backend.Utils.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services.Processor.Interface
{
	public interface IProcessorService
	{
		List<ProcessorModel> Processors { get; }
		ProcessorModel? GetProcessor(int id);
		Task AddProcessor(ProcessorModel model);
		Task UpdateFarmingSpot(ProcessorModel model);
	}
}