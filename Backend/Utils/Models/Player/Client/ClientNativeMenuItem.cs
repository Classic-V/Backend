using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Utils.Enums;

namespace Backend.Utils.Models.Player.Client
{
	public class ClientNativeMenuItem
	{
		public bool Enabled { get; set; } = true;
		public string Label { get; set; }
		public NativeMenuItemType Type { get; set; } = NativeMenuItemType.BUTTON;
		public object? Data { get; set; }
		public object? Value { get; set; }
		public string? CallbackEvent { get; set; }
		public object[]? CallbackArgs { get; set; }
		public bool Close { get; set; } = false;
		public bool IgnoreFilter { get; set; } = false;

		public ClientNativeMenuItem(string label)
		{
			Label = label;
		}
	}
}