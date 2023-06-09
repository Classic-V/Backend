using AltV.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Database
{
	public class ProcessorModel
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public Position Position { get; set; }
		public PositionModel Ped { get; set; } = new();
		public int NeededItem { get; set; }
		public int GetItem { get; set; }
		public int NeededItemAmount { get; set; }
		public int GetItemAmount { get; set; }
		public int Duration { get; set; }
	}
}