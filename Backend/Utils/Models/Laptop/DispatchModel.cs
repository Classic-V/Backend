using AltV.Net.Data;
using Backend.Utils.Enums;

namespace Backend.Utils.Models.Laptop
{
	public class DispatchModel
	{
		private static int IdCounter = 1;
		public int Id { get; set; }
		public int CreatorId { get; set; }
		public string Creator { get; set; }
		public string Message { get; set; }
		public Position Position { get; set; }
		public string Officer { get; set; }
		public string Date { get; set; }
		public DispatchType Type { get; set; }

		public DispatchModel(int creatorId, string creator, string message, Position position, string date, DispatchType type)
		{
			Id = IdCounter++;
			Creator = creator;
			CreatorId = creatorId;
			Message = message;
			Position = position;
			Officer = string.Empty;
			Date = date;
			Type = type;
		}
	}
}