using Backend.Utils.Enums;
using Backend.Utils.Models.Inventory;
using Backend.Utils.Models.Vehicle;

namespace Backend.Utils.Models.Database
{
	public class VehicleModel
	{
		public int Id { get; set; }
		public int InfoModelId { get; set; }
		public TuningModel Tuning { get; set; } = new();
		public int Owner { get; set; } = -1;
		public bool Parked { get; set; } = false;
		public int Garage { get; set; } = -1;
		public PositionModel Position { get; set; } = new();
		public InventoryModel Trunk { get; set; } = new(0, 0, InventoryType.VEHICLE_TRUNK);
		public InventoryModel GloveBox { get; set; } = new(0, 0, InventoryType.VEHICLE_GLOVEBOX);
		public VehicleType Type { get; set; } = VehicleType.TEMPORARY;
		public string Plate { get; set; } = string.Empty;
		public string Note { get; set; } = string.Empty;
		public float Fuel { get; set; } = 100;
	}
}