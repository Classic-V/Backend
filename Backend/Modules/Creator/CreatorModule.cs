using AltV.Net.Async;
using Backend.Controllers.Event.Interface;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Player;
using Backend.Utils.Models.Player.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Modules.Creator
{
	public class CreatorModule : Module<CreatorModule>
	{
		public CreatorModule(IEventController eventController) : base("Creator")
		{
			eventController.OnClient<string>("Server:Creator:Finish", Finish);
		}

		private async void Finish(ClPlayer player, string eventKey, string data)
		{
			if (player.DbModel == null) return;

			await player.SetDimension(0);
			player.DbModel.Customization = JsonConvert.DeserializeObject<CustomizationModel>(data)!;
			player.DbModel.Customization.FinishedCreation = true;
			await player.ShowComponent("Creator", false);
			await player.ShowComponent("Hud", true);
			await player.ApplyCustomization();
			await player.Load();
		}
	}
}