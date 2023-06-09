using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models
{
	public class Ped
	{
		private static int IdCounter = 0;

		public int Id { get; set; }
		public uint Hash { get; set; }
		public float PosX { get; set; }
		public float PosY { get; set; }
		public float PosZ { get; set; }
		public float Heading { get; set; }
		public bool Network { get; set; }
		public bool Freezed { get; set; }
		public bool BScriptHostPed { get; set; }
		public int Dimension { get; set; }

		public Ped(uint hash, float posX, float posY, float posZ, float heading, bool network, bool bScriptHostPed, int dimension)
		{
			Id = ++IdCounter;
			Hash = hash;
			PosX = posX;
			PosY = posY;
			PosZ = posZ;
			Heading = (float)(heading * 180 / Math.PI);
			Network = network;
			Freezed = true;
			BScriptHostPed = bScriptHostPed;
			Dimension = dimension;
		}
	}
}