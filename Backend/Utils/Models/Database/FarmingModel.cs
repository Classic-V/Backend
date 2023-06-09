using AltV.Net.Data;
using Backend.Utils.Models.Farming;
using Backend.Utils.Models.Inventory;

namespace Backend.Utils.Models.Database
{
	public class FarmingModel
	{
		public int Id { get; set; }
		public Position Position { get; set; }
		public uint ObjectHash { get; set; }
		public List<FarmingObjectModel> Objects { get; set; } = new();
		public int Animation { get; set; }
		public int NeededItem { get; set; }
		public int GetItem { get; set; }
		public int MinItemGet { get; set; }
		public int MaxItemGet { get; set; }
	}
}