using AltV.Net.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models
{
	public class PositionModel2
	{
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }
		public float RotX { get; set; }
		public float RotY { get; set; }
		public float RotZ { get; set; }

		[JsonIgnore]
		public Position Position => new(X, Y, Z);
		[JsonIgnore]
		public Rotation Rotation => new(RotX, RotY, RotZ);

		public PositionModel2() { }

		public PositionModel2(float x, float y, float z, float h)
		{
			X = x;
			Y = y;
			Z = z;
			RotX = 0;
			RotY = 0;
			RotZ = h;
		}

		public PositionModel2(Position pos, float h)
		{
			X = pos.X;
			Y = pos.Y;
			Z = pos.Z;
			RotX = 0;
			RotY = 0;
			RotZ = h;
		}

		public PositionModel2(Position pos, Rotation rot)
		{
			X = pos.X;
			Y = pos.Y;
			Z = pos.Z;
			RotX = rot.Pitch;
			RotY = rot.Roll;
			RotZ = rot.Yaw;
		}
	}
}