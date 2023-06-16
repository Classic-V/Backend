using Backend.Utils.Models.Database;

namespace Backend.Controllers.Ban.Interface;

public interface IBanController
{
    Task<bool> IsAccountBanned(int accountId);
    Task<BanModel> GetActiveBan(int accountId);
}