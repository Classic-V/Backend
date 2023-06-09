using AltV.Net.Data;
using Backend.Utils.Models.VehicleShop;

namespace Backend.Utils.Models.Database
{
	public class VehicleShopModel
	{
		public int Id { get; set; }
		public Position Position { get; set; }
		public PositionModel Ped { get; set; } = new();
		public List<VehicleShopItem> Vehicles { get; set; } = new();
		public List<PositionModel> Spawns { get; set; } = new();
	}
}