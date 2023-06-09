namespace Backend.Utils.Models.Player.Customization
{
	public class FaceModel
	{
		public float NoseWidth { get; set; }
		public float NoseHeight { get; set; }
		public float NoseLength { get; set; }
		public float NoseBridge { get; set; }
		public float NosePeak { get; set; }
		public float NoseMovement { get; set; }

		public FaceModel()
		{
			NoseWidth = 0f;
			NoseHeight = 0f;
			NoseLength = 0f;
			NoseBridge = 0f;
			NosePeak = 0f;
			NoseMovement = 0f;
		}

		public FaceModel(float noseWidth, float noseHeight, float noseLength, float noseBridge, float nosePeak, float noseMovement)
		{
			NoseWidth = noseWidth;
			NoseHeight = noseHeight;
			NoseLength = noseLength;
			NoseBridge = noseBridge;
			NosePeak = nosePeak;
			NoseMovement = noseMovement;
		}
	}
}