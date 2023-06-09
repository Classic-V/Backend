namespace Backend.Utils.Models.Team.Client
{
	public class TeamFederalClientData
	{
		public string Name { get; set; }
		public string MeeleWeapon { get; set; }
		public bool Leader { get; set; }
		public bool Bank { get; set; }
		public bool Duty { get; set; }
		public bool SWAT { get; set; }
		public ClientFederalTeam Team { get; set; }

		public TeamFederalClientData(string name, string meeleWeapon, bool leader, bool bank, bool duty, bool sWAT, ClientFederalTeam team)
		{
			Name = name;
			MeeleWeapon = meeleWeapon;
			Leader = leader;
			Bank = bank;
			Duty = duty;
			SWAT = sWAT;
			Team = team;
		}
	}
}