using AltV.Net.Async;
using Backend.Controllers.Blip.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;
using AltV.Net.Elements.Entities;

namespace Backend.Controllers.Blip
{
	public class BlipController : IBlipController
	{
		public BlipController()
		{
			using var ctx = new DBContext();

			new List<BlipModel>(ctx.Blips).ForEach(LoadBlip);
		}

		private async void LoadBlip(BlipModel model)
		{
			var blip = await AltAsync.CreateBlip(BlipType.Destination, model.Position);
			blip.Sprite = (ushort)model.Sprite;
			blip.Color = (byte)model.Color;
			blip.Name = model.Name;
			blip.ShortRange = model.ShortRange;
		}
	}
}