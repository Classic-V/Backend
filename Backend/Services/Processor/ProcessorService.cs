using Backend.Services.Processor.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services.Processor
{
	public class ProcessorService : IProcessorService
	{
		public List<ProcessorModel> Processors { get; private set; } = new();

		public ProcessorService()
		{
			using var ctx = new DBContext();

			Processors = new List<ProcessorModel>(ctx.Processors);
		}

		public ProcessorModel? GetProcessor(int id)
		{
			return Processors.FirstOrDefault(x => x.Id == id);
		}

		public async Task AddProcessor(ProcessorModel model)
		{
			await using var ctx = new DBContext();

			Processors.Add(model);
			await ctx.Processors.AddAsync(model);
			await ctx.SaveChangesAsync();
		}

		public async Task UpdateFarmingSpot(ProcessorModel model)
		{
			using var ctx = new DBContext();

			ctx.Processors.Update(model);
			await ctx.SaveChangesAsync();
		}
	}
}