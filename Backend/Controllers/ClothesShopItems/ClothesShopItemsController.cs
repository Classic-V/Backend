using Backend.Controllers.ClothesShopItems.Interface;
using Backend.Services.ClothesShop.Interface;
using Backend.Services.ClothesShopItems.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models.Database;

namespace Backend.Controllers.ClothesShopItems;

public class ClothesShopItemsController : IClothesShopItemsController
{
    private readonly IClothesShopItemsService _clothesShopItemsService;

    public ClothesShopItemsController(IClothesShopItemsService clothesShopItemsService)
    {
        _clothesShopItemsService = clothesShopItemsService;
    }

    public ClothesShopItemModel? GetClothingItemById(int id)
    {
        return _clothesShopItemsService.GetClothesById(id);
    }

    public List<ClothesShopItemModel?> GetClothingItemsByType(ClothesShopItemType type, bool gender)
    {
        return _clothesShopItemsService.GetClothesByType(type, gender)!;
    }
}