using Backend.Utils.Models.Player.Customization;

namespace Backend.Utils.Models.Player
{
	public class CustomizationModel
	{
		public int Gender { get; set; }
		public ParentsModel Parents { get; set; }
		public FaceModel Face { get; set; }
		public EyeModel Eye { get; set; }
		public HairModel Hair { get; set; }
		public float NeckWidth { get; set; }
		public float LipWidth { get; set; }
		public int Age { get; set; }
		public int MakeUp { get; set; }
		public int MakeupColor { get; set; }
		public float MakeupOpacity { get; set; }
		public int Blush { get; set; }
		public int BlushColor { get; set; }
		public float BlushOpacity { get; set; }
		public int Lipstick { get; set; }
		public int LipstickColor { get; set; }
		public float LipstickOpacity { get; set; }
		public bool FinishedCreation { get; set; }

		public CustomizationModel()
		{
			Gender = 1;
			Parents = new ParentsModel();
			Face = new FaceModel();
			Eye = new EyeModel();
			Hair = new HairModel();
			NeckWidth = 0f;
			LipWidth = 0f;
			Age = 0;
			MakeUp = 0;
			MakeupColor = 0;
			MakeupOpacity = 0;
			Blush = 0;
			BlushColor = 0;
			BlushOpacity = 0;
			Lipstick = 0;
			LipstickColor = 0;
			LipstickOpacity = 0;
			FinishedCreation = false;
		}
	}
}