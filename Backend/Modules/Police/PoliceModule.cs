using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Backend.Controllers.Event.Interface;
using Backend.Data;
using Backend.Modules.Inventory;
using Backend.Utils.Enums;
using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Player.Client;

namespace Backend.Modules.Police
{
	public class PoliceModule : Module<PoliceModule>, IEventColshape
	{
		public static bool SWATState { get; set; } = false;
		private static Position ShopPosition { get; } = new(858.2901f, -1321.2263f, 28.134033f);

		public PoliceModule(IEventController eventController) : base("Police")
		{
			var shape = (ClShape)Alt.CreateColShapeSphere(ShopPosition, 1.2f);
			shape.Id = 1;
			shape.ShapeType = ColshapeType.SWAT_SHOP;
			shape.Size = 1.2f;

			eventController.OnClient<bool>("Server:SWAT:ToggleDuty", ToggleDuty);
			eventController.OnClient<int, int>("Server:SWAT:BuyItem", BuySWATItem);
			eventController.OnClient("Server:SWAT:OpenShop", OpenSWATShop);
		}

		private async void ToggleDuty(ClPlayer player, string eventKey, bool state)
		{
			if(state && ClPlayer.All.Where(x => x.DbModel != null && x.DbModel.Team == player.DbModel.Team && x.DbModel.SwatDuty).Count() >= 5)
			{
				await player.Notify("SWAT DIENST", "Es sind bereits 5 Personen aus deiner Fraktion im SWAT Dienst!", NotificationType.ERROR);
				return;
			}

			player.DbModel.SwatDuty = state;
			await player.Notify("SWAT DIENST", $"Du hast den SWAT Dienst {(state ? "betreten" : "verlassen")}.", NotificationType.INFO);
			foreach (var swatMember in ClPlayer.All.Where(x => x != player && x.DbModel != null && x.DbModel.Team > 0 && x.DbModel.Team < 3 && x.DbModel.TeamStoragePermission))
				await swatMember.Notify("SWAT DIENST", $"{player.Name} hat den SWAT Dienst {(state ? "betreten" : "verlassen")}.", NotificationType.INFO);
		}

		private async void BuySWATItem(ClPlayer player, string eventKey, int itemId, int amount)
		{
			var item = InventoryModule.GetItemBase(itemId);
			if (item == null) return;

			player.DbModel.Inventory.AddItem(item, amount);
			await player.Notify("SWAT SHOP", $"Du hast {amount}x {item.Name} aus dem Schrank geholt.", NotificationType.INFO);
		}

		private void OpenSWATShop(ClPlayer player, string eventKey)
		{
			if (player.DbModel.Team < 1 || player.DbModel.Team > 2 || !player.DbModel.Duty || !player.DbModel.TeamStoragePermission || player.Position.Distance(ShopPosition) > 1.5f) return;

			List<ClientNativeMenuItem> items = null!;

			if (player.DbModel.SwatDuty)
			{
				items = new()
				{
					new("SWAT Dienst verlassen", true, "Server:SWAT:ToggleDuty", false),
					new("SWAT Einsatzkleidung", false, "Server:SWAT:BuyItem", 30, 1),
					new("SWAT Schutzweste", false, "Server:SWAT:BuyItem", 32, 1),
				};
			}
			else
			{
				items = new()
				{
					new("SWAT Dienst betreten", true, "Server:SWAT:ToggleDuty", true),
				};
			}

			player.ShowNativeMenu(true, new("SWAT Ausrüstung", items));
		}

		public Task OnColshape(ClShape shape, IEntity entity, bool entered)
		{
			if (entity.Type != BaseObjectType.Player || shape.ShapeType != ColshapeType.SWAT_SHOP) return Task.CompletedTask;

			var player = (ClPlayer)entity;
			if (player.DbModel == null || player.DbModel.Team < 1 || player.DbModel.Team > 2 || !player.DbModel.TeamStoragePermission) return Task.CompletedTask;

			player.SetInteraction(Interactions.KEY_E, entered ? "SWAT_SHOP" : Interactions.NONE);

			return Task.CompletedTask;
		}
	}
}