using Backend.Utils.Models.Database;
using Backend.Utils.Models.Shops;

namespace Backend.Controllers.Wardrobe.Interface;

public interface IWardrobeController
{
    void LoadWardrobe(WardrobeModel model);
    Task<WardrobeModel?> GetWardrobe(int id);
    Task<int> CreateWardrobe(WardrobeModel model);
}