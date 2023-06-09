using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Player.Client
{
	public class ClientNativeMenuItem
	{
		public string Label { get; set; }
		public string CallbackEvent { get; set; }
		public object[] CallbackArgs { get; set; }
		public bool Close { get; set; }

		public ClientNativeMenuItem(string label, bool close, string callbackEvent, params object[] callbackArgs)
		{
			Label = label;
			CallbackEvent = callbackEvent;
			CallbackArgs = callbackArgs;
			Close = close;
		}
	}
}