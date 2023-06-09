using Backend.Utils.Models.Database;

namespace Backend.Controllers.Outfit.Interface;

public interface IOutfitController
{
    Task<OutfitModel?> GetOutfit(int id);
    Task<List<OutfitModel>?> GetOutfits(int playerId);
    Task<int> CreateOutfit(OutfitModel model);
    Task DeleteOutfit(OutfitModel model);
}