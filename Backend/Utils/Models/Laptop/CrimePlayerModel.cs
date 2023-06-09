namespace Backend.Utils.Models.Laptop
{
	public class CrimePlayerModel
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public CrimePlayerModel(int id, string name)
		{
			Id = id;
			Name = name;
		}
	}
}