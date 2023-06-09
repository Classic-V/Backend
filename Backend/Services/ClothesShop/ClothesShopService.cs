using Backend.Services.ClothesShop.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;

namespace Backend.Services.ClothesShop;

public class ClothesShopService : IClothesShopService
{
    public List<ClothesShopModel> ClothesShop { get; private set; }

    public ClothesShopService()
    {
        var context = new DBContext();

        ClothesShop = new List<ClothesShopModel>(context.ClothesShop);
    }

    public ClothesShopModel? GetClothesShopById(int id)
    {
        return ClothesShop.FirstOrDefault(x => x.Id == id);
    }

    public async Task AddClothesShop(ClothesShopModel clothesShop)
    {
        var context = new DBContext();
        context.ClothesShop.Add(clothesShop);
        ClothesShop.Add(clothesShop);
        await context.SaveChangesAsync();
    }

    public async Task UpdateClothesShop(ClothesShopModel clothesShop)
    {
        var context = new DBContext();
        context.ClothesShop.Update(clothesShop);
        await context.SaveChangesAsync();
    }
}