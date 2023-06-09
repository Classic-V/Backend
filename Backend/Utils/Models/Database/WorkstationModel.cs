using AltV.Net.Data;
using Backend.Utils.Models.Workstation;

namespace Backend.Utils.Models.Database
{
	public class WorkstationModel
	{
		public int Id { get; set; }
		public Position Position { get; set; }
		public PositionModel Ped { get; set; } = new();
		public List<WorkstationItemModel> Items { get; set; } = new();
	}
}