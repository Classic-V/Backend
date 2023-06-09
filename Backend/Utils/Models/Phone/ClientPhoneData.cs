namespace Backend.Utils.Models.Phone
{
	public class ClientPhoneData
	{
		public int Background { get; set; }
		public int Team { get; set; }
		public bool TeamLeader { get; set; }
		public int Business { get; set; }
		public bool BusinessLeader { get; set; }

		public ClientPhoneData(int background, int team, bool teamLeader, int business, bool businessLeader)
		{
			Background = background;
			Team = team;
			TeamLeader = teamLeader;
			Business = business;
			BusinessLeader = businessLeader;
		}
	}
}