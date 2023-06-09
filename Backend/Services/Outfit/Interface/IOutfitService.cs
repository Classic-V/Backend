using Backend.Utils.Models.Database;

namespace Backend.Services.Outfit.Interface;

public interface IOutfitService
{
    List<OutfitModel> Outfits { get; }
    Task AddOutfit(OutfitModel model);
    Task DeleteOutfit(OutfitModel model);
    Task<OutfitModel?> GetOutfit(int id);
}