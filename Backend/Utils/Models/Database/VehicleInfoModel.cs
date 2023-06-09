using Backend.Utils.Enums;

namespace Backend.Utils.Models.Database
{
	public class VehicleInfoModel
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public uint Hash { get; set; } = 0;
		public int TrunkWeight { get; set; }
		public int TrunkSlots { get; set; }
		public int GloveBoxWeight { get; set; }
		public int GloveBoxSlots { get; set; }
		public int MaxFuel { get; set; }
		public GarageVehicleType GarageType { get; set; }
		public int Tax { get; set; }
		public int Seats { get; set; }
	}
}