using AltV.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Database
{
	public class DropModel
	{
		public int Id { get; set; }
		public string Location { get; set; } = string.Empty;
		public PositionModel Position { get; set; } = new();
		public PositionModel LootCrate1 { get; set; } = new();
		public PositionModel LootCrate2 { get; set; } = new();
		public PositionModel LootCrate3 { get; set; } = new();
	}
}