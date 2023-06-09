using Backend.Services.Outfit.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;

namespace Backend.Services.Outfit;

public class OutfitService : IOutfitService
{
    public List<OutfitModel> Outfits { get; }

    public OutfitService()
    {
        var ctx = new DBContext();
        Outfits = new List<OutfitModel>(ctx.Outfits);
    }

    public async Task AddOutfit(OutfitModel model)
    {
        Outfits.Add(model);

        var ctx = new DBContext();
        ctx.Outfits.Add(model);
        await ctx.SaveChangesAsync();
    }

    public async Task DeleteOutfit(OutfitModel model)
    {
        Outfits.Remove(model);

        var ctx = new DBContext();
        ctx.Outfits.Remove(model);
        await ctx.SaveChangesAsync();
    }

    public Task<OutfitModel?> GetOutfit(int id)
    {
        return Task.FromResult(Outfits.FirstOrDefault(x => x.Id == id));
    }
}