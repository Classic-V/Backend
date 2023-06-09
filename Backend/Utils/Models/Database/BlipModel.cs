using AltV.Net.Data;

namespace Backend.Utils.Models.Database
{
	public class BlipModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public Position Position { get; set; }
		public int Sprite { get; set; }
		public int Color { get; set; }
		public bool ShortRange { get; set; }
	}
}