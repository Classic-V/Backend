using Backend.Utils.Enums;
using Backend.Utils.Models.Database;

namespace Backend.Services.ClothesShopItems.Interface;

public interface IClothesShopItemsService
{
    List<ClothesShopItemModel> ClothesShopItemList { get; }

    ClothesShopItemModel GetClothesById(int id);
    List<ClothesShopItemModel> GetClothesByType(ClothesShopItemType type, bool gender);
    Task AddClothesItem(ClothesShopItemModel clothesItem);
}