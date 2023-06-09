using Backend.Modules.Inventory;
using Newtonsoft.Json;
using System.Text;

namespace Backend.Utils
{
	public static class DiscordWebhook
	{
		private static readonly HttpClient _client = new();

		public static async Task SendItems()
		{
			var msg = "";

			InventoryModule.ItemModels.OrderBy(x => x.Id).ToList().ForEach(x => msg += $"{x.Id} - {x.Name}\n");

			var hook = new
			{
				username = "PGRP",
				embeds = new List<object>
				{
					new
					{
						title = "Items",
						description = msg,
						color = int.Parse("00b7ff", System.Globalization.NumberStyles.HexNumber)
					}
				}
			};

			var content = new StringContent(JsonConvert.SerializeObject(hook), Encoding.UTF8, "application/json");

			await _client.PostAsync("https://discord.com/api/webhooks/1088557616508174336/sSRDvz06qI656KrCA-KRkxHaXfT3BG_YDu1EPHooZk7xfCVz9eUFeSbcOw4IXEtKxmtm", content);
		}
	}
}