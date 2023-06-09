using AltV.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Team.Client
{
	public class TeamClientLaboratoryData
	{
		public Position Position { get; set; }
		public int Fuel { get; set; }

		public TeamClientLaboratoryData(Position position, int fuel)
		{
			Position = position;
			Fuel = fuel;
		}
	}
}