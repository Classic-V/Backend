using AltV.Net;
using AltV.Net.Elements.Entities;
using Backend.Controllers.ClothesShop.Interface;
using Backend.Services.ClothesShop.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models.ClothesShop;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;

namespace Backend.Controllers.ClothesShop;

public class ClothesShopController : IClothesShopController
{
    private readonly IClothesShopService _clothesShopService;

    public ClothesShopController(IClothesShopService clothesShopService)
    {
        _clothesShopService = clothesShopService;

        _clothesShopService.ClothesShop.ForEach(LoadShops);
    }

    private void LoadShops(ClothesShopModel shopModel)
    {
        var blip = Alt.CreateBlip(BlipType.Destination, shopModel.Position.Position);
        blip.Dimension = 0;
        blip.Sprite = 73;
        blip.Color = 4;
        blip.Name = "Kleidungsladen";
        blip.ShortRange = true;

        var colShape = (ClShape) Alt.CreateColShapeSphere(shopModel.Position.Position, 4f);
        colShape.Dimension = 0;
        colShape.Size = 4f;
        colShape.ShapeType = ColshapeType.CLOTHES_SHOP;
        colShape.Id = shopModel.Id;
    }

    public Task<ClothesShopModel?> GetShop(int id)
    {
        return Task.FromResult(_clothesShopService.GetClothesShopById(id));
    }
}