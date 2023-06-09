using Backend.Utils.Enums;

namespace Backend.Utils.Models.Input
{
	public class InputDataModel
	{
		public string Title { get; set; }
		public string Message { get; set; }
		public int Type { get; set; }
		public string CallbackEvent { get; set; }
		public object[] CallbackArgs { get; set; }

		public InputDataModel(string title, string message, InputType type, string callback, params object[] args)
		{
			Title = title;
			Message = message;
			Type = (int)type;
			CallbackEvent = callback;
			CallbackArgs = args;
		}
	}
}