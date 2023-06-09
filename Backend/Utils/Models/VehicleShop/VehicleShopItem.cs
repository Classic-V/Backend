using Backend.Utils.Models.Database;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Utils.Models.VehicleShop
{
	public class VehicleShopItem
	{
		public int VehicleInfoId { get; set; }
		public PositionModel2 Position { get; set; } = new();
		public int Price { get; set; }

		[JsonIgnore]
		[NotMapped]
		public VehicleInfoModel VehicleInfo { get; set; } = null!;
	}
}