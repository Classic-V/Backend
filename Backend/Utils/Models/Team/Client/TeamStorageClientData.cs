using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Team.Client
{
	public class TeamStorageClientData
	{
		public int Level { get; set; }
		public int Weight { get; set; }
		public int MaxWeight { get; set; }
		public int Slots { get; set; }
		public int MaxSlots { get; set; }
		public int Drugs { get; set; }

		public TeamStorageClientData(int level, int weight, int maxWeight, int slots, int maxSlots, int drugs)
		{
			Level = level;
			Weight = weight;
			MaxWeight = maxWeight;
			Slots = slots;
			MaxSlots = maxSlots;
			Drugs = drugs;
		}
	}
}