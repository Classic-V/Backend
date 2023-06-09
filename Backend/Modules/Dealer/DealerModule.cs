using AltV.Net.Elements.Entities;
using Backend.Controllers.Dealer.Interface;
using Backend.Controllers.Event.Interface;
using Backend.Data;
using Backend.Utils.Enums;
using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Modules.Dealer
{
	public class DealerModule : Module<DealerModule>, IEventColshape
	{
		private readonly IDealerController _dealerController;

		public DealerModule(IEventController eventController, IDealerController dealerController) : base("Dealer")
		{
			_dealerController = dealerController;

			eventController.OnClient("Server:Dealer:Open", OpenMenu);
			eventController.OnClient<int, int>("Server:Dealer:SellItem", SellItem);
		}

		public Task OnColshape(ClShape shape, IEntity entity, bool entered)
		{
			if (entity.Type != BaseObjectType.Player || shape.ShapeType != ColshapeType.DEALER) return Task.CompletedTask;

			var player = (ClPlayer)entity;
			player.SetInteraction(Interactions.KEY_E, entered ? Interactions.E_DEALER : Interactions.NONE);
			return Task.CompletedTask;
		}

		private async void OpenMenu(ClPlayer player, string eventKey)
		{
			if (player.DbModel == null || player.DbModel.Team < 1 || player.CurrentShape == null || player.CurrentShape.ShapeType != Utils.Enums.ColshapeType.DEALER) return;

			await _dealerController.OpenDealerMenu(player, player.CurrentShape.Id);
		}

		private async void SellItem(ClPlayer player, string eventKey, int id, int itemId)
		{
			if (player.DbModel == null || player.DbModel.Team < 1 || player.CurrentShape == null || player.CurrentShape.ShapeType != Utils.Enums.ColshapeType.DEALER) return;

			await _dealerController.SellItem(player, id, itemId);
		}
	}
}