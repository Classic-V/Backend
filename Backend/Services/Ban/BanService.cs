using Backend.Services.Ban.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;

namespace Backend.Services.Ban;

public class BanService : IBanService
{
    public List<BanModel> Bans { get; private set; }

    public BanService()
    {
        var ctx = new DBContext();
        Bans = new List<BanModel>(ctx.Bans);
    }
    
    public Task<bool> IsAccountBanned(int id)
    {
        return Task.FromResult(Bans.Find(x => x.AccountId == id && x.Active && (x.Permanent || x.BannedTill > DateTime.Now)) != null);
    }
    
    public Task<BanModel> GetActiveBan(int id)
    {
        return Task.FromResult(Bans.Find(x => x.AccountId == id && x.Active && (x.Permanent || x.BannedTill > DateTime.Now))!);
    }

    public async Task AddBan(BanModel model)
    {
        await using var ctx = new DBContext();

        Bans.Add(model);
        await ctx.Bans.AddAsync(model);
        await ctx.SaveChangesAsync();
    }
}