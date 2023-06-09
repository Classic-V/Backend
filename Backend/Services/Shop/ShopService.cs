using Backend.Services.Shop.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;

namespace Backend.Services.Shop;

public class ShopService : IShopService
{
    public List<ShopModel> Shops { get; private set; }

    public ShopService()
    {
        using var ctx = new DBContext();
        Shops = new List<ShopModel>(ctx.Shops);
    }

    public async Task AddShop(ShopModel model)
    {
        Shops.Add(model);

        var ctx = new DBContext();
        ctx.Shops.Add(model);
        await ctx.SaveChangesAsync();
    }

    public async Task UpdateShop(ShopModel model)
    {
        var ctx = new DBContext();
        ctx.Shops.Update(model);
        await ctx.SaveChangesAsync();
    }

    public async Task DeleteShop(ShopModel model)
    {
        Shops.Remove(model);

        var ctx = new DBContext();
        ctx.Shops.Remove(model);
        await ctx.SaveChangesAsync();
    }

    public Task<ShopModel?> GetShop(int id)
    {
        return Task.FromResult(Shops.FirstOrDefault(x => x.Id == id));
    }
}