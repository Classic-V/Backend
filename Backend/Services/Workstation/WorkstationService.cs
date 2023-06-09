using Backend.Services.Workstation.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;

namespace Backend.Services.Workstation
{
	public class WorkstationService : IWorkstationService
	{
		public List<WorkstationModel> Workstations { get; } = new();

		public WorkstationService()
		{
			using var ctx = new DBContext();

			Workstations = new List<WorkstationModel>(ctx.Workstations);
		}

		public Task<WorkstationModel?> GetWorkstation(int id)
		{
			return Task.FromResult(Workstations.FirstOrDefault(x => x.Id == id));
		}

		public async Task AddWorkstation(WorkstationModel model)
		{
			await using var ctx = new DBContext();

			Workstations.Add(model);
			ctx.Workstations.Add(model);
			await ctx.SaveChangesAsync();
		}
	}
}