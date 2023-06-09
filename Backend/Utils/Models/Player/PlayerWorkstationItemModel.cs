namespace Backend.Utils.Models.Player
{
	public class PlayerWorkstationItemModel
	{
		public int WorkstationId { get; set; }
		public int OutputItemId { get; set; }
		public int OutputItemAmount { get; set; }
		public string Date { get; set; } = string.Empty;
		public int Ticks { get; set; }
		public int Duration { get; set; }

		public PlayerWorkstationItemModel() { }

		public PlayerWorkstationItemModel(int workstation, int outputItemId, int outputItemAmount, string date, int ticks, int duration)
		{
			WorkstationId = workstation;
			OutputItemId = outputItemId;
			OutputItemAmount = outputItemAmount;
			Date = date;
			Ticks = ticks;
			Duration = duration;
		}
	}
}