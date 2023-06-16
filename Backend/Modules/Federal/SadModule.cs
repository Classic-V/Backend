using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Backend.Controllers.Event.Interface;
using Backend.Data;
using Backend.Modules.Inventory;
using Backend.Services.FederalLicense.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Player.Client;

namespace Backend.Modules.Federal
{
    public class SadModule : Module<SadModule>, IEventColshape
    {
        private readonly IFederalLicenseService _federalLicenseService;
        private static readonly Position shopPosition = new(2459.565f, -384.05273f, 93.309204f);

        public SadModule(IEventController eventController, IFederalLicenseService federalLicenseService) : base("Sad")
        {
            _federalLicenseService = federalLicenseService;

            var shape = (ClShape)Alt.CreateColShapeSphere(shopPosition, 2f);
            shape.Id = 1;
            shape.ShapeType = ColshapeType.SAD_SHOP;
            shape.Dimension = 0;
            shape.Size = 2f;

            eventController.OnClient("Server:SAD:OpenShop", OpenShop);
            eventController.OnClient<bool>("Server:SAD:ToggleDuty", ToggleDuty);
            eventController.OnClient<int, int>("Server:SAD:BuyItem", BuyItem);
        }

        private async void OpenShop(ClPlayer player, string eventKey)
        {
            if (player.DbModel == null || player.DbModel.Team != 2 || player.DbModel.TeamRank < 4) return;

            var licences = await _federalLicenseService.GetLicense(player.DbModel.Id);
            if (licences == null) return;

            if (!licences.SadLicense)
            {
                foreach (var direction in ClPlayer.All.Where(x => x != player && x.DbModel != null && x.DbModel.Team == 2 && x.DbModel.TeamRank >= 10))
                    await direction.Notify("SAD DIENST", $"{player.Name} hat versucht in den SAD Dienst zugehen.", NotificationType.INFO);

                return;
            }

            var items = new List<ClientNativeMenuItem>();
            if (!player.DbModel.SadDuty)
            {
                items.Add(new ClientNativeMenuItem("SAD Dienst betreten") { Close = true, CallbackEvent = "Server:SAD:ToggleDuty", CallbackArgs = new object[] { !player.DbModel.SadDuty } });
            } 
            else 
            {
                items.Add(new ClientNativeMenuItem("SAD Dienst beenden") { Close = true, CallbackEvent = "Server:SAD:ToggleDuty", CallbackArgs = new object[] { !player.DbModel.SadDuty } });
                items.Add(new ClientNativeMenuItem("SAD Einsatzkleidung") { Close = true, CallbackEvent = "Server:SAD:BuyItem", CallbackArgs = new object[] { 9991, 1 } });
                items.Add(new ClientNativeMenuItem("SAD Schutzweste") { Close = true, CallbackEvent = "Server:SAD:BuyItem", CallbackArgs = new object[] { 9999, 1 } });
            }

            var menu = new ClientNativeMenu("SAD SHOP", items);
            await player.ShowNativeMenu(true, menu);
        }

        private async void ToggleDuty(ClPlayer player, string eventKey, bool state)
        {
            var licences = await _federalLicenseService.GetLicense(player.DbModel.Id);
            if (licences == null) return;
            if (!licences.SadLicense) return;

            player.DbModel.SadDuty = state;

            await player.Notify("SAD Dienst", $"Du hast den SAD Dienst {(state ? "betreten" : "verlassen")}.", NotificationType.INFO);
            foreach (var sadMember in ClPlayer.All.Where(x => x != player && x.DbModel != null && x.DbModel.Team == 2 && licences.SadLicense))
                await sadMember.Notify("SAD DIENST", $"{player.Name} hat den SAD Dienst {(state ? "betreten" : "verlassen")}.", NotificationType.INFO);
        }

        private async void BuyItem(ClPlayer player, string eventKey, int itemId, int amount)
        {
            var item = InventoryModule.GetItemBase(itemId);
            if (item == null) return;

            player.DbModel.Inventory.AddItem(item, amount);
            await player.Notify("SAD SHOP", $"Du hast {amount}x {item.Name} aus dem Schrank geholt.", NotificationType.INFO);
        }

        public async Task OnColshape(ClShape shape, IEntity entity, bool entered)
        {
            if (shape == null || entity.Type != BaseObjectType.Player || shape.ShapeType != ColshapeType.SAD_SHOP) return;

            ClPlayer player = (ClPlayer)entity;
            if (player == null || player.DbModel == null) return;

            player.SetInteraction(Interactions.KEY_E, entered ? Interactions.E_SAD_OPEN_SHOP : Interactions.NONE);
        }
    }
}
