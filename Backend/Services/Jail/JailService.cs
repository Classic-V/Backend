using Backend.Services.Jail.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;

namespace Backend.Services.Jail;

public class JailService : IJailService
{
    public List<JailModel> Jails { get; private set; }

    public JailService()
    {
        var ctx = new DBContext();
        Jails = new List<JailModel>(ctx.Jail);
    }

    public Task<JailModel?> GetJail(int id)
    {
        return Task.FromResult(Jails.FirstOrDefault(x => x.Id == id));
    }

    public async Task AddJail(JailModel model)
    {
        Jails.Add(model);

        var ctx = new DBContext();
        ctx.Jail.Add(model);
        await ctx.SaveChangesAsync();
    }

    public async Task UpdateJail(JailModel model)
    {
        var ctx = new DBContext();
        ctx.Jail.Update(model);
        await ctx.SaveChangesAsync();
    }
}