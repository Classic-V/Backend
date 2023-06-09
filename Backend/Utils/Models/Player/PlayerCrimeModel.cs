namespace Backend.Utils.Models.Player
{
	public class PlayerCrimeModel
	{
		public int CrimeId { get; set; }
		public string Date { get; set; } = string.Empty;
		public string Creator { get; set; } = string.Empty;

		public PlayerCrimeModel() { }
		public PlayerCrimeModel(int crimeId, string date, string creator)
		{
			CrimeId = crimeId;
			Date = date;
			Creator = creator;
		}
	}
}