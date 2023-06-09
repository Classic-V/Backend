namespace Backend.Utils.Models.Database
{
	public class CrimeModel
	{
		public int Id { get; set; }
		public int Group { get; set; }
		public string Label { get; set; } = string.Empty;
		public int JailTime { get; set; }
		public int Fine { get; set; }
	}
}