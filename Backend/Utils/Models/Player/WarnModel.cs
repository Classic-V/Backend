namespace Backend.Utils.Models.Player
{
	public class WarnModel
	{
		public string Reason { get; set; } = string.Empty;
		public int CreatorId { get; set; }
		public DateTime Date { get; set; }
		
		public WarnModel() { }

		public WarnModel(string reason, int creatorId, DateTime date)
		{
			Reason = reason;
			CreatorId = creatorId;
			Date = date;
		}
	}
}