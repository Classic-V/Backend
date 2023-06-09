using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Player.Client
{
	public class ClientHudData
	{
		public int Money { get; set; }
		public int Starvation { get; set; }
		public int Hydration { get; set; }
		public float Strength { get; set; }

		public ClientHudData(int money, int starvation, int hydration, float strength)
		{
			Money = money;
			Starvation = starvation;
			Hydration = hydration;
			Strength = strength;
		}
	}
}