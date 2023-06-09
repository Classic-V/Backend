using Backend.Services.ClothesShopItems.Interface;
using Backend.Utils;
using Backend.Utils.Enums;
using Backend.Utils.Models.Database;

namespace Backend.Services.ClothesShopItems;

public class ClothesShopItemsService : IClothesShopItemsService
{
    public List<ClothesShopItemModel> ClothesShopItemList { get; private set; }

    public ClothesShopItemsService()
    {
        var ctx = new DBContext();
        ClothesShopItemList = new List<ClothesShopItemModel>(ctx.ClothesShopItem);
    }

    public ClothesShopItemModel GetClothesById(int id)
    {
        return ClothesShopItemList.FirstOrDefault(x => x.Id == id)!;
    }

    public List<ClothesShopItemModel> GetClothesByType(ClothesShopItemType type, bool gender)
    {
        return ClothesShopItemList.Where(x => x.Type == type && x.Gender == gender).ToList();
    }

    public async Task AddClothesItem(ClothesShopItemModel clothesItem)
    {
        ClothesShopItemList.Add(clothesItem);

        var ctx = new DBContext();
        ctx.ClothesShopItem.Add(clothesItem);
        await ctx.SaveChangesAsync();
    }
}