using Backend.Utils.Models.Entities;

namespace Backend.Utils.Models.Laptop
{
	public class UnitModel
	{
		private static int IdCounter = 1;

		public int Id { get; set; }
		public string Name { get; set; }
		public int Vehicle { get; set; }
		public List<UnitMemberModel> Players { get; set; }
		public int Team { get; set; }

		public UnitModel(string name, int vehicle, List<UnitMemberModel> players, int team)
		{
			Id = IdCounter++;
			Name = name;
			Vehicle = vehicle;
			Players = players;
			Team = team;
		}
	}
}