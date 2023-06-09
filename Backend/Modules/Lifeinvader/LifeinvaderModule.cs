using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Backend.Controllers.Event.Interface;
using Backend.Data;
using Backend.Utils.Enums;
using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Input;
using Backend.Utils.Models.Lifeinvader;
using Backend.Utils.Streamer;
using Newtonsoft.Json;

namespace Backend.Modules.Lifeinvader
{
	public class LifeinvaderModule : Module<LifeinvaderModule>, IEventColshape
	{
		private readonly List<LifeinvaderPost> _posts = new();
		private readonly Position _pos = new(-1082.1099f, -247.52966f, 37.75537f);
		private readonly PositionModel _pedPos = new(-1083.3363f, -245.92088f, 36.75537f, -2.6221168f);

		public LifeinvaderModule(IEventController eventController) : base("Lifeinvader")
		{
			var shape = (ClShape)Alt.CreateColShapeSphere(_pos, 2f);
			shape.Id = 1;
			shape.ShapeType = ColshapeType.LIFEINVADER;
			shape.Size = 2f;

			PedStreamer.AddObject(new Ped(0x5389A93C, _pedPos.X, _pedPos.Y, _pedPos.Z, _pedPos.H, true, true, 0));

			var blip = Alt.CreateBlip((byte)BlipType.Destination, _pos);
			blip.Name = "Lifeinvader";
			blip.Sprite = 77;
			blip.ShortRange = true;
			blip.Color = 1;

			eventController.OnClient<string>("Server:Lifeinvader:CreatePost", CreatePost);
			eventController.OnClient("Server:Lifeinvader:Open", Open);
			eventController.OnClient("Server:Lifeinvader:GetData", GetData);
		}

		private async void CreatePost(ClPlayer player, string eventKey, string content)
		{
			if (player.DbModel == null || player.Position.Distance(_pos) > 3) return;

			var price = content.Length * 15;

			if (player.DbModel.Money < price)
			{
				await player.Notify("LIFEINVADER", $"Du hast nicht genug Geld dabei! (${price})", NotificationType.ERROR);
				return;
			}

			if (DateTime.Now < player.LastLifeinvaderPost.AddMinutes(15))
			{
				await player.Notify("LIFEINVADER", $"Du kannst nur jede 15 Minuten eine Werbung schalten!", NotificationType.ERROR);
				return;
			}

			player.LastLifeinvaderPost = DateTime.Now;
			await player.RemoveMoney(price);

			_posts.Add(new LifeinvaderPost(
				player.DbModel.Id,
				content,
				player.DbModel.Id,
				DateTime.Now.ToString("HH:mm")));

			ClPlayer.LoggedIn.ForEach(target =>
			{
                target.Notify("Lifeinvader", "Es wurde eine neue Anzeige erstellt!", NotificationType.INFO);

                if (target.DbModel.AdminRank > AdminRank.PLAYER)
					target.Notify("Lifeinvader", $"Spieler {player.Name}({player.DbModel.Id}): {content}", NotificationType.INFO);
			});
		}

		private void Open(ClPlayer player, string eventKey)
		{
			if (player.DbModel == null || player.Position.Distance(_pos) > 3) return;

			player.ShowComponent("Input", true, JsonConvert.SerializeObject(new InputDataModel(
				"Lifeinvader Anzeige schalten",
				"Was möchtest du veröffentlichen? (Pro Zeichen $15)",
				InputType.TEXT,
				"Server:Lifeinvader:CreatePost")));
		}

		private void GetData(ClPlayer player, string eventKey)
		{
			player.EmitBrowser("Phone:Lifeinvader:SetData", JsonConvert.SerializeObject(_posts));
		}

		public Task OnColshape(ClShape shape, IEntity entity, bool entered)
		{
			if (entity.Type != BaseObjectType.Player || shape.ShapeType != ColshapeType.LIFEINVADER) return Task.CompletedTask;

			var player = (ClPlayer)entity;
			player.SetInteraction(Interactions.KEY_E, entered ? Interactions.E_LIFEINVADER : Interactions.NONE);
			return Task.CompletedTask;
		}
	}
}