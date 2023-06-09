using AltV.Net.Data;

namespace Backend.Utils.Models
{
	public class Object
	{
		private static int IdCounter = 0;

		public int Id { get; set; }
		public uint Hash { get; set; }
		public double PosX { get; set; }
		public double PosY { get; set; }
		public double PosZ { get; set; }
		public double RotX { get; set; }
		public double RotY { get; set; }
		public double RotZ { get; set; }
		public bool Network { get; set; }
		public bool Dynamic { get; set; }
		public int Dimension { get; set; }
		public object? GameObject { get; set; } = null;
		
		public Object()
		{
		}

		public Object(uint hash, Position pos, Rotation rot, bool netowrk, bool dynamic, int dimension)
		{
			Id = IdCounter++;
			Hash = hash;
			PosX = pos.X;
			PosY = pos.Y;
			PosZ = pos.Z;
			RotX = rot.Roll;
			RotY = rot.Pitch;
			RotZ = rot.Yaw;
			Network = netowrk;
			Dynamic = dynamic;
			Dimension = dimension;
		}
	}
}