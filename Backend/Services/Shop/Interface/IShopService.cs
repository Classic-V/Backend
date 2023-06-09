using Backend.Utils.Models.Database;

namespace Backend.Services.Shop.Interface;

public interface IShopService
{
    List<ShopModel> Shops { get; }
    Task AddShop(ShopModel model);
    Task UpdateShop(ShopModel model);
    Task DeleteShop(ShopModel model);
    Task<ShopModel?> GetShop(int id);
}