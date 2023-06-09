using Backend.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Player.Client
{
	public class ClientNotificationModel
	{
		public string Title { get; set; }
		public string Message { get; set; }
		public int Type { get; set; }
		public int Duration { get; set; }

		public ClientNotificationModel(string title, string message, NotificationType type, int duration)
		{
			Title = title;
			Message = message;
			Type = (int)type;
			Duration = duration;
		}
	}
}