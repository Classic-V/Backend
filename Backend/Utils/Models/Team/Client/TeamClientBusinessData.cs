using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Team.Client
{
	public class TeamClientBusinessData
	{
		public int Online { get; set; }
		public int Leader { get; set; }
		public int Members { get; set; }

		public TeamClientBusinessData(int online, int leader, int members)
		{
			Online = online;
			Leader = leader;
			Members = members;
		}
	}
}