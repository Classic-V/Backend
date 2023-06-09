using AltV.Net;
using Backend.Controllers.ClothesShop.Interface;
using Backend.Controllers.ClothesShopItems.Interface;
using Backend.Controllers.Event.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.ClothesShop;
using Backend.Utils.Models.ClothesShop.Client;
using Backend.Utils.Models.Entities;
using Newtonsoft.Json;

namespace Backend.Modules.ClothesShop;

public class ClothesShopModule : Module<ClothesShopModule>
{
    private readonly IClothesShopController _clothesShopController;
    private readonly IClothesShopItemsController _clothesShopItemsController;

    public ClothesShopModule(IClothesShopController clothesShopController, IClothesShopItemsController clothesShopItemsController, IEventController eventController) : base("ClothesShop")
    {
        _clothesShopController = clothesShopController;
		_clothesShopItemsController = clothesShopItemsController;

		eventController.OnClient<int>("Server:ClothesShop:Open", OpenShop);
		eventController.OnClient<int, int, int, string>("Server:ClothesShop:Try", Try);
		eventController.OnClient("Server:ClothesShop:ResetClothes", Reset);
		eventController.OnClient<string>("Server:ClothesShop:Buy", BuyItem);
    }

	private void Try(ClPlayer player, string eventKey, int comp, int draw, int tex, string dlc)
	{
		if (dlc == "") player.SetClothes((byte)comp, (ushort)draw, (byte)tex, 0);
		else player.SetDlcClothes((byte)comp, (ushort)draw, (byte)tex, 0, Alt.Hash(dlc));
	}

	private async void Reset(ClPlayer player, string eventKey) => await player.ApplyClothes();

	private async void BuyItem(ClPlayer player, string eventKey, string itemIds)
    {
        if (player.CurrentShape == null) return;

        var id = player.CurrentShape.Id;

        var shop = await _clothesShopController.GetShop(id);
        if (shop == null || player.Position.Distance(shop.Position.Position) > 10) return;

        int price = 0;

        List<int> ids = JsonConvert.DeserializeObject<List<int>>(itemIds)!;
        ids.ForEach(async itemId =>
        {
            var clothingData = _clothesShopItemsController.GetClothingItemById(itemId);
			if (clothingData == null) return;

			if (player.DbModel.WardrobeClothes.FirstOrDefault(x => x.Component == clothingData.Component && x.Texture == clothingData.Texture && x.Dlc == clothingData.Dlc) != null)
			{
				await player.Notify("BEKLEIDUNGSLADEN", $"Das Kleidungsstück {clothingData.Name} ist bereits in deinem Kleiderschrank", NotificationType.INFO);
				return;
			}

            price += clothingData.Price;
        });

        if (player.DbModel.Money < price)
        {
            await player.Notify("BEKLEIDUNGSLADEN", $"Du hast nicht genug Geld.", NotificationType.ERROR);
            return;
        }

        ids.ForEach(async itemId =>
        {
            var clothingData = _clothesShopItemsController.GetClothingItemById(itemId);
            if (clothingData == null) return;
            
            await player.RemoveMoney(price);

            player.DbModel.WardrobeClothes.Add(clothingData);

            if (clothingData.IsProp) SetProp(player, new ClothingModel(clothingData.Component, clothingData.Drawable, clothingData.Texture, Alt.Hash(clothingData.Dlc)));
            else SetClothing(player, new ClothingModel(clothingData.Component, clothingData.Drawable, clothingData.Texture, Alt.Hash(clothingData.Dlc)));
        });

        await player.ApplyClothes();
		await player.Notify("BEKLEIDUNGSLADEN", $"Du hast {ids.Count} Kleidungsstücke für ${price} erworben.", NotificationType.INFO);
    }

    private async void OpenShop(ClPlayer player, string eventKey, int id)
    {
        var shop = await _clothesShopController.GetShop(id);
        if (shop == null) return;

        var gender = Convert.ToBoolean(player.DbModel.Customization.Gender);
        var items = _clothesShopItemsController.GetClothingItemsByType((ClothesShopItemType)shop.Type, gender);
		if (items == null) return;

		player.Rotation = new(0, 0, shop.Position.H);
		await player.ShowComponent("ClothesShop", true, JsonConvert.SerializeObject(new ClientClothesShopModel(items, (int)shop.Type)));
	}

    private void SetClothing(ClPlayer player, ClothingModel model)
    {
        switch(model.Component)
        {
            case 1:
                player.DbModel.Clothes.Mask = model;
                break;
			case 11:
				player.DbModel.Clothes.Top = model;
				break;
			case 8:
				player.DbModel.Clothes.Undershirt = model;
				break;
			case 3:
				player.DbModel.Clothes.Body = model;
				break;
			case 4:
				player.DbModel.Clothes.Leg = model;
				break;
			case 6:
				player.DbModel.Clothes.Shoe = model;
				break;
			case 9:
				player.DbModel.Clothes.Armor = model;
				break;
			case 5:
				player.DbModel.Clothes.Bag = model;
				break;
			case 7:
				player.DbModel.Clothes.Accessories = model;
				break;
		}
    }

	private void SetProp(ClPlayer player, ClothingModel model)
	{
		switch (model.Component)
		{
			case 0:
				player.DbModel.Clothes.Hat = model;
				break;
			case 1:
				player.DbModel.Clothes.Glasses = model;
				break;
			case 2:
				player.DbModel.Clothes.Ears = model;
				break;
			case 6:
				player.DbModel.Clothes.Watch = model;
				break;
			case 7:
				player.DbModel.Clothes.Bracelet = model;
				break;
		}
	}
}