using Backend.Utils.Models.Database;

namespace Backend.Services.Ban.Interface;

public interface IBanService
{
    List<BanModel> Bans { get; }

    Task<bool> IsAccountBanned(int accountId);
    Task<BanModel> GetActiveBan(int accountId);
    Task AddBan(BanModel model);
}