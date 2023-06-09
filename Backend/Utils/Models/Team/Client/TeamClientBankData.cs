using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Team.Client
{
	public class TeamClientBankData
	{
		public string Name { get; set; }
		public int Amount { get; set; }
		public string Date { get; set; }
		public bool Type { get; set; }

		public TeamClientBankData(string name, int amount, string date, bool type)
		{
			Name = name;
			Amount = amount;
			Date = date;
			Type = type;
		}
	}
}