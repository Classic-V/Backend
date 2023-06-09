using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Gangwar.Client
{
	public class GangwarHudModel
	{
		public GangwarHudTeamModel Attacker { get; set; } = new();
		public GangwarHudTeamModel Defender { get; set; } = new();
		public int Time { get; set; }

		public GangwarHudModel() { }

		public GangwarHudModel(GangwarHudTeamModel attacker, GangwarHudTeamModel defender, int time)
		{
			Attacker = attacker;
			Defender = defender;
			Time = time;
		}
	}
}