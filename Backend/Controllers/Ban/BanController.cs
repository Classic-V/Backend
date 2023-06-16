using Backend.Controllers.Ban.Interface;
using Backend.Services.Ban.Interface;
using Backend.Utils.Models.Database;

namespace Backend.Controllers.Ban;

public class BanController : IBanController
{
    private readonly IBanService _banService;

    public BanController(IBanService banService)
    {
        _banService = banService;
    }

    public async Task<bool> IsAccountBanned(int accountId)
    {
        return await _banService.IsAccountBanned(accountId);
    }
    
    public async Task<BanModel> GetActiveBan(int accountId)
    {
        return await _banService.GetActiveBan(accountId);
    }
}