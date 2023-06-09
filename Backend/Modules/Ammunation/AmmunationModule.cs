using AltV.Net.Elements.Entities;
using Backend.Controllers.Ammunation.Interface;
using Backend.Controllers.Event.Interface;
using Backend.Modules.Inventory;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Ammunation;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Shops;
using Newtonsoft.Json;

namespace Backend.Modules.Ammunation;

public class AmmunationModule : Module<AmmunationModule>
{
    private readonly IAmmunationController _ammunationController;

    public AmmunationModule(IAmmunationController ammunationController, IEventController eventController) : base("Ammunation")
    {
        _ammunationController = ammunationController;

        eventController.OnClient("Server:Ammunation:Open", OpenAmmunation);
        eventController.OnClient<int, string>("Server:Ammunnation:BuyItems", BuyItems);
    }

    private async void BuyItems(ClPlayer player, string eventKey, int id, string items)
    {
        var shop = await _ammunationController.GetAmmunation(id);
        if (shop == null  || player.Position.Distance(shop.Position) > 20) return;

        await player.ShowComponent("Shop", false);

        var shopItems = JsonConvert.DeserializeObject<List<ShopBuyItemModel>>(items)!;
        if (shopItems.Count < 1) return;

        int price = 0;
        float weight = 0;
        float slots = 0;

        shopItems.ForEach(async item =>
        {
            var data = await _ammunationController.GetWeaponItemData(shop.Id, item.Id);
            if (data == null) return;

            var model = InventoryModule.GetItemBase(data.ItemId);
            if (data == null! || model == null) return;

            price += item.Amount * data.Price;
            weight += model.Weight * item.Amount;
            slots += (float)Math.Ceiling((decimal)item.Amount / model.MaxAmount);
        });

        if (weight > player.DbModel.Inventory.MaxWeight - player.DbModel.Inventory.GetInventoryWeight() || slots > player.DbModel.Inventory.GetFreeSlots())
        {
            await player.Notify(shop.Name, "Du hast nicht genug Platz für den Einkauf.", NotificationType.ERROR);
            return;
        }

        if (player.DbModel.Money < price)
        {
            await player.Notify(shop.Name, "Du hast nicht genug Geld für den Einkauf.", NotificationType.ERROR);
            return;
        }

        await player.RemoveMoney(price);

        shopItems.ForEach(async item =>
        {
            var itemData = await _ammunationController.GetWeaponItemData(shop.Id, item.Id);
            player.DbModel.Inventory.AddItem(InventoryModule.GetItemBase(itemData!.ItemId), item.Amount);
        });

        await player.Notify(shop.Name, "Vielen Dank für ihren Einkauf.", NotificationType.INFO);
    }

    private async void OpenAmmunation(ClPlayer player, string eventKey)
    {
        if (player.CurrentShape == null || player.CurrentShape.ShapeType != ColshapeType.SHOP) return;

        var shop = await _ammunationController.GetAmmunation(player.CurrentShape.Id);
        if (shop == null) return;

        await player.ShowComponent("Shop", true, JsonConvert.SerializeObject(new WeaponShopDataModel(shop.Id, shop.Name, shop.Items)));
    }
}