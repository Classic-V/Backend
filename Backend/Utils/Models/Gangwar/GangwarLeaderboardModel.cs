using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Gangwar
{
	public class GangwarLeaderboardModel
	{
		public string AttackerName { get; set; }
		public int AttackerPoints { get; set; }
		public string DefenderName { get; set; }
		public int DefenderPoints { get; set; }
		public List<LeaderboardModel> Players { get; set; }

		public GangwarLeaderboardModel(string attackerName, int attackerPoints, string defenderName, int defenderPoints, List<LeaderboardModel> players)
		{
			AttackerName = attackerName;
			AttackerPoints = attackerPoints;
			DefenderName = defenderName;
			DefenderPoints = defenderPoints;
			Players = players;
		}
	}
}