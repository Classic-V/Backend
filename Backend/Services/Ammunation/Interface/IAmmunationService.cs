using Backend.Utils.Models.Database;

namespace Backend.Services.Ammunation.Interface;

public interface IAmmunationService
{
    List<AmmunationModel> Ammunations { get; }
    Task<AmmunationModel> AddAmmunation(AmmunationModel model);
    Task UpdateAmmunation(AmmunationModel model);
    Task DeleteAmmunation(AmmunationModel model);
    Task<AmmunationModel?> GetAmmunation(int id);
}