namespace Backend.Utils.Models.Team.Client
{
	public class TeamClientData
	{
		public string Name { get; set; }
		public string MeeleWeapon { get; set; }
		public bool Leader { get; set; }
		public bool Bank { get; set; }
		public ClientTeam Team { get; set; }
		public TeamClientGangwarData? Gangwar { get; set; }

		public TeamClientData(string name, string meeleWeapon, bool leader, bool bank, ClientTeam team, TeamClientGangwarData? gangwar)
		{
			Name = name;
			MeeleWeapon = meeleWeapon;
			Leader = leader;
			Bank = bank;
			Team = team;
			Gangwar = gangwar;
		}
	}
}