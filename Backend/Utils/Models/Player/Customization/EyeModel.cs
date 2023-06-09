namespace Backend.Utils.Models.Player.Customization
{
	public class EyeModel
	{
		public float EyeHeight { get; set; }
		public float EyeWidth { get; set; }
		public float Eye { get; set; }
		public int EyeColor { get; set; }
		public int Eyebrow { get; set; }
		public int EyebrowColor { get; set; }

		public EyeModel()
		{
			EyeHeight = 0f;
			EyeWidth = 0f;
			Eye = 0f;
			EyeColor = 0;
			Eyebrow = 0;
			EyebrowColor = 0;
		}

		public EyeModel(float eyeHeight, float eyeWidth, float eye, int eyeColor, int eyebrow, int eyebrowColor)
		{
			EyeHeight = eyeHeight;
			EyeWidth = eyeWidth;
			Eye = eye;
			EyeColor = eyeColor;
			Eyebrow = eyebrow;
			EyebrowColor = eyebrowColor;
		}
	}
}