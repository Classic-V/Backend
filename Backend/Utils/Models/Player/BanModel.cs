namespace Backend.Utils.Models.Player
{
	public class BanModel
	{
		public bool Active { get; set; }
		public string Reason { get; set; }
		public DateTime Date { get; set; }
		public string Admin { get; set; }

		public BanModel()
		{
			Active = false;
			Reason = string.Empty;
			Date = DateTime.Now;
			Admin = string.Empty;
		}

		public BanModel(bool active, string reason, DateTime date, string admin)
		{
			Active = active;
			Reason = reason;
			Date = date;
			Admin = admin;
		}
	}
}