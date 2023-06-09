using AltV.Net;
using Backend.Controllers.Dealer.Interface;
using Backend.Controllers.Team.Interface;
using Backend.Services.Dealer.Interface;
using Backend.Services.Team.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Utils.Models.Player.Client;
using Backend.Modules.Inventory;
using Backend.Utils.Streamer;
using AltV.Net.Enums;

namespace Backend.Controllers.Dealer
{
	public class DealerController : IDealerController
	{
		private readonly IDealerService _dealerService;
		private readonly ITeamService _teamService;

		public DealerController(IDealerService dealerService, ITeamService teamService)
		{
			_dealerService = dealerService;
			_teamService = teamService;

			var random = new Random();

			for (var i = 0; i <= 4; i++)
				LoadDealer(_dealerService.Dealer[random.Next(_dealerService.Dealer.Count)]);

		}

		public Task LoadDealer(DealerModel model)
		{
			ClShape shape = (ClShape)Alt.CreateColShapeSphere(model.Position.Position, 1.5f);
			shape.Id = model.Id;
			shape.ShapeType = ColshapeType.DEALER;
			shape.Size = 1.5f;

			PedStreamer.AddObject(
				new Utils.Models.Ped(
					(uint)PedModel.Abigail,
					model.Position.X,
					model.Position.Y,
					model.Position.Z - 1,
					model.Position.H,
					false,
					false,
					0)
				{
					Freezed = true
				});

			return Task.CompletedTask;
		}

		public async Task OpenDealerMenu(ClPlayer player, int id)
		{
			if (player.DbModel == null || player.DbModel.Team < 1) return;

			var dealer = _dealerService.GetDealer(id);
			if (dealer == null) return;

			var team = await _teamService.GetTeam(player.DbModel.Team);
			if (team == null || team.Type == TeamType.POLICE) return;

			await player.ShowNativeMenu(true, new ClientNativeMenu(
				$"Dealer #{id}",
				new List<ClientNativeMenuItem>()
				{
					new ClientNativeMenuItem($"Kokain Verkaufen (${dealer.CocainePrice})", true, "Server:Dealer:SellItem", id, 301),
					new ClientNativeMenuItem($"Kokain Kiste Verkaufen (${dealer.CocaineBoxPrice})", true, "Server:Dealer:SellItem", id, 401),
					new ClientNativeMenuItem($"Weed Verkaufen (${dealer.WeedPrice})", true, "Server:Dealer:SellItem", id, 302),
					new ClientNativeMenuItem($"Weed Kiste Verkaufen (${dealer.WeedBoxPrice})", true, "Server:Dealer:SellItem", id, 402),
					new ClientNativeMenuItem($"Goldbarren Verkaufen (${dealer.GoldPrice})", true, "Server:Dealer:SellItem", id, 10000),
					new ClientNativeMenuItem($"Juwelen Verkaufen (${dealer.JewelPrice})", true, "Server:Dealer:SellItem", id, 10001),
				}));
		}

		public async Task SellItem(ClPlayer player, int id, int itemId)
		{
			if (player.DbModel == null || player.DbModel.Team < 1) return;

			var dealer = _dealerService.GetDealer(id);
			if (dealer == null) return;

			var team = await _teamService.GetTeam(player.DbModel.Team);
			if (team == null) return;

			var price = GetItemPrice(dealer, itemId);
			if (price == 0) return;

			var model = InventoryModule.GetItemBase(itemId)!;
			var amount = player.DbModel.Inventory.Items.Where(x => x.Model.Id == itemId).Sum(x => x.Amount);
			if(amount < 1)
			{
				await player.Notify("DEALER", "Du hast diesen Gegenstand nicht dabei!", NotificationType.ERROR);
				return;
			}

			player.DbModel.Inventory.RemoveItem(model, amount);
			await player.AddMoney(price * amount);
			await player.Notify("DEALER", $"Du hast {amount}x {model.Name} für ${price * amount} verkauft!", NotificationType.SUCCESS);
		}

		private int GetItemPrice(DealerModel model, int itemId)
		{
			switch (itemId)
			{
				case 301: return model.CocainePrice;
				case 401: return model.CocaineBoxPrice;
				case 302: return model.WeedPrice;
				case 402: return model.WeedBoxPrice;
				case 10000: return model.GoldPrice;
				case 10001: return model.JewelPrice;
			}

			return 0;
		}
	}
}