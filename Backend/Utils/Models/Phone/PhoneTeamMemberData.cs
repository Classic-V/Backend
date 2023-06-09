namespace Backend.Utils.Models.Phone
{
	public class PhoneTeamMemberData
	{
		public int Id { get; }
		public string Name { get; }
		public int Phone { get; }
		public int Rank { get; }
		public bool Leader { get; }

		public PhoneTeamMemberData(int id, string name, int phone, int rank, bool leader)
		{
			Id = id;
			Name = name;
			Phone = phone;
			Rank = rank;
			Leader = leader;
		}
	}
}