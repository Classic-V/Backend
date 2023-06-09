namespace Backend.Utils.Models.Player.Customization
{
	public class ParentsModel
	{
		public int Mother { get; set; }
		public int Father { get; set; }
		public float Similarity { get; set; }
		public float SkinColor { get; set; }

		public ParentsModel()
		{
			Mother = 21;
			Father = 0;
			Similarity = 0.5f;
			SkinColor = 0.5f;
		}

		public ParentsModel(int mother, int father, float similarity, float skinColor)
		{
			Mother = mother;
			Father = father;
			Similarity = similarity;
			SkinColor = skinColor;
		}
	}
}