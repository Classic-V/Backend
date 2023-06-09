namespace Backend.Utils.Models.Laptop
{
	public class UnitMemberModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Rank { get; set; }

		public UnitMemberModel(int id, string name, int rank)
		{
			Id = id;
			Name = name;
			Rank = rank;
		}
	}
}