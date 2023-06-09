using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Player
{
	public class DrugState
	{
		public bool Active { get; set; }
		public DateTime UsedTime { get; set; }

		public DrugState(bool active, DateTime time)
		{
			Active = active;
			UsedTime = time;
		}
	}
}