namespace Backend.Utils.Models.Team.Client
{
	public class TeamClientMemberData
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Rank { get; set; }
		public bool Online { get; set; }
		public bool Admin { get; set; }
		public bool Storage { get; set; }
		public bool Bank { get; set; }
		public string LastOnline { get; set; }
		public string JoinDate { get; set; }

		public TeamClientMemberData(int id, string name, int rank, bool online, bool admin, bool storage, bool bank, string lastOnline, string joinDate)
		{
			Id = id;
			Name = name;
			Rank = rank;
			Online = online;
			Admin = admin;
			Storage = storage;
			Bank = bank;
			LastOnline = lastOnline;
			JoinDate = joinDate;
		}
	}
}