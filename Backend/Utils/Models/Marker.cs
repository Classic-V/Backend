using AltV.Net.Data;

namespace Backend.Utils.Models
{
	public class Marker
	{
		private static int IdCounter = 0;

		public int Id { get; set; }
		public int Type { get; set; } = 0;
		public float PosX { get; set; } = 0;
		public float PosY { get; set; } = 0;
		public float PosZ { get; set; } = 72;
		public float DirX { get; set; } = 0;
		public float DirY { get; set; } = 0;
		public float DirZ { get; set; } = 0;
		public float RotX { get; set; } = 0;
		public float RotY { get; set; } = 0;
		public float RotZ { get; set; } = 0;
		public float ScaleX { get; set; } = 1;
		public float ScaleY { get; set; } = 1;
		public float ScaleZ { get; set; } = 1;
		public int ColorR { get; set; } = 0;
		public int ColorG { get; set; } = 0;
		public int ColorB { get; set; } = 0;
		public int Alpha { get; set; } = 255;
		public float DrawRange { get; set; } = 0;
		public bool BobUpDown { get; set; } = false;
		public bool FaceCamera { get; set; } = false;
		public bool Rotate { get; set; } = false;
		public int Dimension { get; set; } = 0;

		public Marker(int type, Position pos, Rgba color, float drawRange, int dimension)
		{
			Id = ++IdCounter;
			Type = type;
			PosX = pos.X;
			PosY = pos.Y;
			PosZ = pos.Z;
			ColorR = color.R;
			ColorG = color.G;
			ColorB = color.B;
			Alpha = color.A;
			DrawRange = drawRange;
			Dimension = dimension;
		}

		public Marker(int type, Position pos, Position scale, Rgba color, float drawRange, int dimension)
		{
			Id = ++IdCounter;
			Type = type;
			PosX = pos.X;
			PosY = pos.Y;
			PosZ = pos.Z;
			ScaleX = scale.X;
			ScaleY = scale.Y;
			ScaleZ = scale.Z;
			ColorR = color.R;
			ColorG = color.G;
			ColorB = color.B;
			Alpha = color.A;
			DrawRange = drawRange;
			Dimension = dimension;
		}

		public Marker(int type, Position pos, Position scale, Rgba color, float drawRange, bool bobUpDown, bool faceCamera, bool rotate, int dimension)
		{
			Id = ++IdCounter;
			Type = type;
			PosX = pos.X;
			PosY = pos.Y;
			PosZ = pos.Z;
			ScaleX = scale.X;
			ScaleY = scale.Y;
			ScaleZ = scale.Z;
			ColorR = color.R;
			ColorG = color.G;
			ColorB = color.B;
			Alpha = color.A;
			DrawRange = drawRange;
			BobUpDown = bobUpDown;
			FaceCamera = faceCamera;
			Rotate = rotate;
			Dimension = dimension;
		}
	}
}