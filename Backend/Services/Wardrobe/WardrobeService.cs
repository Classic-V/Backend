using Backend.Services.Wardrobe.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;

namespace Backend.Services.Wardrobe;

public class WardrobeService : IWardrobeService
{
    public List<WardrobeModel> Wardrobes { get; private set; }

    public WardrobeService()
    {
        var context = new DBContext();
        Wardrobes = new List<WardrobeModel>(context.Wardrobes);
    }

    public async Task AddWardrobe(WardrobeModel model)
    {
        Wardrobes.Add(model);

        var context = new DBContext();
        context.Wardrobes.Add(model);
        await context.SaveChangesAsync();
    }

    public async Task UpdateWardrobe(WardrobeModel model)
    {
        var context = new DBContext();
        context.Wardrobes.Update(model);
        await context.SaveChangesAsync();
    }

    public async Task DeleteWardrobe(WardrobeModel model)
    {
        Wardrobes.Remove(model);

        var context = new DBContext();
        context.Wardrobes.Remove(model);
        await context.SaveChangesAsync();
    }

    public Task<WardrobeModel?> GetWardrobe(int id)
    {
        return Task.FromResult(Wardrobes.FirstOrDefault(x => x.Id == id));
    }
}