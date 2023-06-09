using Backend.Utils.Enums;
using Backend.Utils.Models.Database;

namespace Backend.Controllers.ClothesShopItems.Interface;

public interface IClothesShopItemsController
{
    ClothesShopItemModel GetClothingItemById(int id);
    List<ClothesShopItemModel>? GetClothingItemsByType(ClothesShopItemType type, bool gender);
}