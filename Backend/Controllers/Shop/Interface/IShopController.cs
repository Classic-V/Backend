using Backend.Utils.Models.Database;
using Backend.Utils.Models.Shops;

namespace Backend.Controllers.Shop.Interface;

public interface IShopController
{
    void LoadShop(ShopModel model);
	Task<ShopModel?> GetShop(int id);
    Task<ShopItemModel?> GetItemData(int shopId, int id);
    Task<int> CreateShop(ShopModel model);
    Task SaveShops();
}