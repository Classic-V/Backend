using Backend.Utils.Models.Database;

namespace Backend.Services.FFA.Interface;

public interface IFFAService
{
    List<FFAModel> FFA { get; }
    Task<FFAModel?> GetFFA(int id);
    Task<FFAModel?> GetFFA(string name);
    Task Create(int maxplayers, string password, string name, uint firstWeapon, uint secondWeapon);
    Task Close(int id);
}