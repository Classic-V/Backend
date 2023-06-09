using Backend.Utils.Models.ClothesShop;
using Backend.Utils.Models.Database;

namespace Backend.Controllers.ClothesShop.Interface;

public interface IClothesShopController
{
    Task<ClothesShopModel?> GetShop(int id);
}