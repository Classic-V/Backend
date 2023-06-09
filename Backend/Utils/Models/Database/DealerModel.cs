using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Utils.Models.Database
{
	public class DealerModel
	{
		public int Id { get; set; }
		public PositionModel Position { get; set; } = new();

		[NotMapped]
		public int CocainePrice { get; set; } = new Random().Next(100, 400);
		[NotMapped]
		public int CocaineBoxPrice { get; set; } = new Random().Next(3000, 5000);

		[NotMapped]
		public int WeedPrice { get; set; } = new Random().Next(100, 400);
		[NotMapped]
		public int WeedBoxPrice { get; set; } = new Random().Next(3000, 5000);

		[NotMapped]
		public int GoldPrice { get; set; } = new Random().Next(1000, 2500);
		[NotMapped]
		public int JewelPrice { get; set; } = new Random().Next(500, 1500);
	}
}