using Backend.Utils.Enums;
using Backend.Utils.Models.Database;

namespace Backend.Services.ClothesShop.Interface;

public interface IClothesShopService
{
    List<ClothesShopModel> ClothesShop { get; }

    ClothesShopModel? GetClothesShopById(int id);
    Task AddClothesShop(ClothesShopModel clothesShop);
    Task UpdateClothesShop(ClothesShopModel clothesShop);
}