using AltV.Net.Data;
using Backend.Utils.Enums;

namespace Backend.Utils.Models.Database
{
	public class GarageModel
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public Position Position { get; set; } = new();
		public PositionModel PedPosition { get; set; } = new();
		public List<PositionModel> Spawns { get; set; } = new();
		public GarageType Type { get; set; } = GarageType.PUBLIC;
		public int Owner { get; set; } = -1;
		public GarageVehicleType VehicleType { get; set; } = GarageVehicleType.SMALL;
	}
}