using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Gangwar
{
	public class LeaderboardModel
	{
		public string Name { get; set; }
		public int Kills { get; set; }
		public int Deaths { get; set; }

		public LeaderboardModel(string name, int kills, int deaths)
		{
			Name = name;
			Kills = kills;
			Deaths = deaths;
		}
	}
}