using Backend.Utils.Models.Database;

namespace Backend.Services.Workstation.Interface
{
	public interface IWorkstationService
	{
		List<WorkstationModel> Workstations { get; }
		Task<WorkstationModel?> GetWorkstation(int id);
		Task AddWorkstation(WorkstationModel model);
	}
}