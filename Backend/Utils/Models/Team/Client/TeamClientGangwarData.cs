using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Team.Client
{
	public class TeamClientGangwarData
	{
		public string Name { get; set; }
		public string Enemy { get; set; }

		public TeamClientGangwarData()
		{
		}

		public TeamClientGangwarData(string name, string enemy)
		{
			Name = name;
			Enemy = enemy;
		}
	}
}