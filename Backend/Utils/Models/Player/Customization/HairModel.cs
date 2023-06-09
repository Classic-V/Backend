namespace Backend.Utils.Models.Player.Customization
{
	public class HairModel
	{
		public int Hair { get; set; }
		public int HairColor { get; set; }
		public int HairColor2 { get; set; }
		public int Beard { get; set; }
		public int BeardColor { get; set; }
		public float BeardOpacity { get; set; }

		public HairModel()
		{
			Hair = 0;
			HairColor = 0;
			HairColor2 = 0;
			Beard = 0;
			BeardColor = 0;
			BeardOpacity = 0;
		}

		public HairModel(int hair, int hairColor, int hairColor2, int beard, int beardColor, float beardOpacity)
		{
			Hair = hair;
			HairColor = hairColor;
			HairColor2 = hairColor2;
			Beard = beard;
			BeardColor = beardColor;
			BeardOpacity = beardOpacity;
		}
	}
}