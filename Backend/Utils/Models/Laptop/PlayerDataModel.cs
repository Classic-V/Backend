using Backend.Utils.Models.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Laptop
{
	public class PlayerDataModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int House { get; set; }
		public int Warehouse { get; set; }
		public string Team { get; set; }
		public string Description { get; set; }
		public int Phone { get; set; }
		public List<PlayerCrimeModel> Crimes { get; set; }

		public PlayerDataModel(int id, string name, int house, int warehouse, string team, string description, int phone, List<PlayerCrimeModel> crimes)
		{
			Id = id;
			Name = name;
			House = house;
			Warehouse = warehouse;
			Team = team;
			Description = description;
			Phone = phone;
			Crimes = crimes;
		}
	}
}