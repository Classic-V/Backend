namespace Backend.Utils.Models.Lifeinvader
{
	public class LifeinvaderPost
	{
		private static int IdCounter = 1;

		public int Id { get; set; }
		public int CreatorId { get; set; }
		public string Content { get; set; }
		public int Contact { get; set; }
		public string Date { get; set; }

		public LifeinvaderPost(int creatorId, string content, int contact, string date)
		{
			Id = IdCounter++;
			CreatorId = creatorId;
			Content = content;
			Contact = contact;
			Date = date;
		}
	}
}