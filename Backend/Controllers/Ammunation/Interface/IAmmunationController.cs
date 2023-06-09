using Backend.Utils.Models.Ammunation;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Shops;

namespace Backend.Controllers.Ammunation.Interface;

public interface IAmmunationController
{
    void LoadAmmunation(AmmunationModel model);
    Task<AmmunationModel?> GetAmmunation(int id);
    Task<WeaponShopItemModel?> GetWeaponItemData(int shopId, int id);
    Task<int> CreateAmmunation(AmmunationModel model);
}