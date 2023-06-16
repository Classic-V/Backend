using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Player.Client
{
	public class ClientNativeMenu
	{
		public string Title { get; set; }
		public string Subtitle { get; set; }
		public List<ClientNativeMenuItem> Items { get; set; }

		public ClientNativeMenu(string title, List<ClientNativeMenuItem> items, string subtitle = "")
		{
			Title = title;
			Items = items;
			Subtitle = subtitle;
		}
	}
}