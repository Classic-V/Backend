using Backend.Utils.Models.Database;

namespace Backend.Controllers.Jail.Interface;

public interface IJailController
{
    void LoadJail(JailModel model);
    Task<JailModel?> GetJail(int id);
    Task<int> GetPlayerJailDuration(int id);
    Task<int> GetPlayerJailFine(int id);
    Task<bool> PlayerHasJailCrimes(int id);
}