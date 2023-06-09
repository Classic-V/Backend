using Backend.Controllers.Barber.Interface;
using Backend.Controllers.Event.Interface;
using Backend.Services.Barber.Interface;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Player.Customization;
using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Modules.Barber
{
    public class BarberModule : Module<BarberModule>
    {
        private readonly IBarberController _barberController;
        private readonly IBarberService _barberService;

        public BarberModule(IEventController eventController, IBarberController barberController, IBarberService barberService) : base("Barber")
        {
            _barberController = barberController;
            _barberService = barberService;

            eventController.OnClient<int>("Server:Barber:Open", Open);
            eventController.OnClient<int, int>("Server:Barber:Buy", Buy);
            eventController.OnClient<int, int>("Server:Barber:ChangeHairColor", ChangeHairColor);
            eventController.OnClient<int, int>("Server:Barber:ChangeBeardColor", ChangeBeardColor);
        }

        private async void Open(ClPlayer player, string eventKey, int shopId)
        {
            if (player.DbModel == null || shopId <= 0) return;

            var barber = await _barberService.GetBarber(shopId);
            if (barber == null) return;

            List<BarberItemModel> items = _barberService.BarberItems.Where(x => x.ShopId == barber.Id).ToList();
            if (items == null) return;

            await player.ShowComponent("Barber", true, JsonConvert.SerializeObject(items));
        }

        private async void Buy(ClPlayer player, string eventKey, int shopId, int itemId)
        {
            if (player.DbModel == null || shopId <= 0) return;

            var barber = await _barberService.GetBarber(shopId);
            if (barber == null) return;

            var item = await _barberService.GetBarberItem(itemId);
            if (item == null) return;

            if (player.DbModel.Money < item.Price)
            {
                await player.Notify("Friseur", "Du hast nicht genug Geld!", Utils.Enums.NotificationType.ERROR);
                return;
            }

            player.DbModel.Money -= item.Price;
            await player.SetClothing(2, item.DrawableId, 0, 0);
            player.DbModel.Customization.Hair = new HairModel(item.DrawableId, player.DbModel.Customization.Hair.HairColor, player.DbModel.Customization.Hair.HairColor2, player.DbModel.Customization.Hair.Beard, player.DbModel.Customization.Hair.BeardColor, player.DbModel.Customization.Hair.BeardOpacity);
            await player.Notify("Friseur", "Du hast dir eine Frisur gekauft!", Utils.Enums.NotificationType.SUCCESS);

        }

        private async void ChangeHairColor(ClPlayer player, string eventKey, int primaryHairColor, int secondaryHairColor)
        {
            if (player.DbModel == null || primaryHairColor < 0) return;

            if (player.DbModel.Money < 10)
            {
                await player.Notify("Friseur", "Du hast nicht genug Geld!", Utils.Enums.NotificationType.ERROR);
                return;
            }

            player.DbModel.Customization.Hair = new HairModel(player.DbModel.Customization.Hair.Hair, primaryHairColor, secondaryHairColor, player.DbModel.Customization.Hair.Beard, player.DbModel.Customization.Hair.BeardColor, player.DbModel.Customization.Hair.BeardOpacity);
            await player.ApplyCustomization();
            await player.Notify("Friseur", "Du hast deine Haare verändert!", Utils.Enums.NotificationType.INFO);
            player.DbModel.Money -= 10;
        }

        private async void ChangeBeardColor(ClPlayer player, string eventKey, int beardColor, int beardOpacity)
        {
            if (player.DbModel == null || beardColor < 0) return;

            if (player.DbModel.Money < 10)
            {
                await player.Notify("Friseur", "Du hast nicht genug Geld!", Utils.Enums.NotificationType.ERROR);
                return;
            }

            player.DbModel.Customization.Hair = new HairModel(player.DbModel.Customization.Hair.Hair, player.DbModel.Customization.Hair.HairColor, player.DbModel.Customization.Hair.HairColor2, player.DbModel.Customization.Hair.Beard, beardColor, beardOpacity);
            await player.ApplyCustomization();
            await player.Notify("Friseur", "Du hast dein Bart verändert!", Utils.Enums.NotificationType.INFO);
            player.DbModel.Money -= 10;
        }
    }
}
