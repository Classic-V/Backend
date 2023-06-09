using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Gangwar.Client
{
	public class GangwarHudTeamModel
	{
		public string Name { get; set; } = string.Empty;
		public string Label { get; set; } = string.Empty;
		public int Points { get; set; }

		public GangwarHudTeamModel() { }

		public GangwarHudTeamModel(string name, string label, int points)
		{
			Name = name;
			Label = label;
			Points = points;
		}
	}
}