using Backend.Utils.Models.Database;

namespace Backend.Services.Jail.Interface;

public interface IJailService
{
    List<JailModel> Jails { get; }
    Task<JailModel?> GetJail(int id);
    Task AddJail(JailModel model);
    Task UpdateJail(JailModel model);
}