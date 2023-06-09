using Backend.Services.Ammunation.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;

namespace Backend.Services.Ammunation;

public class AmmunationService : IAmmunationService
{
    public List<AmmunationModel> Ammunations { get; private set; }

    public AmmunationService()
    {
        var ctx = new DBContext();
        Ammunations = new List<AmmunationModel>(ctx.Ammunation);
    }

    public async Task<AmmunationModel> AddAmmunation(AmmunationModel model)
    {
        Ammunations.Add(model);

        var ctx = new DBContext();
        ctx.Ammunation.Add(model);
        await ctx.SaveChangesAsync();

        return model;
    }

    public async Task UpdateAmmunation(AmmunationModel model)
    {
        var ctx = new DBContext();
        ctx.Ammunation.Update(model);
        await ctx.SaveChangesAsync();
    }

    public async Task DeleteAmmunation(AmmunationModel model)
    {
        Ammunations.Remove(model);

        var ctx = new DBContext();
        ctx.Ammunation.Remove(model);
        await ctx.SaveChangesAsync();
    }

    public Task<AmmunationModel?> GetAmmunation(int id)
    {
        return Task.FromResult(Ammunations.FirstOrDefault(x => x.Id == id));
    }
}