using AltV.Net.Data;
using Newtonsoft.Json;

namespace Backend.Utils.Models
{
	public class PositionModel
	{
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }
		public float H { get; set; }

		[JsonIgnore]
		public Position Position => new((float)X, (float)Y, (float)Z);

		public PositionModel() {}

		public PositionModel(float x, float y, float z, float h)
		{
			X = x;
			Y = y;
			Z = z;
			H = h;
		}

		public PositionModel(Position pos, float h)
		{
			X = pos.X;
			Y = pos.Y;
			Z = pos.Z;
			H = h;
		}
	}
}