using Backend.Utils.Models.Database;

namespace Backend.Services.Wardrobe.Interface;

public interface IWardrobeService
{
    List<WardrobeModel> Wardrobes { get; }
    Task AddWardrobe(WardrobeModel model);
    Task UpdateWardrobe(WardrobeModel model);
    Task DeleteWardrobe(WardrobeModel model);
    Task<WardrobeModel?> GetWardrobe(int id);
}