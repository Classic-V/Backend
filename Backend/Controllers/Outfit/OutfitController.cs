using Backend.Controllers.Outfit.Interface;
using Backend.Services.Outfit.Interface;
using Backend.Utils.Models.Database;

namespace Backend.Controllers.Outfit;

public class OutfitController : IOutfitController
{
    private readonly IOutfitService _outfitService;

    public OutfitController(IOutfitService outfitService)
    {
        _outfitService = outfitService;
    }

    public async Task<OutfitModel?> GetOutfit(int id)
    {
        return await _outfitService.GetOutfit(id);
    }

    public Task<List<OutfitModel>?> GetOutfits(int playerId)
    {
        var list = new List<OutfitModel>();

        _outfitService.Outfits.Where(x => x.PlayerId == playerId && x != null).ToList().ForEach(item =>
        {
            list.Add(item);
        });

        return Task.FromResult(list)!;
    }

    public async Task<int> CreateOutfit(OutfitModel model)
    {
        await _outfitService.AddOutfit(model);

        return model.Id;
    }

    public async Task DeleteOutfit(OutfitModel model)
    {
        await _outfitService.DeleteOutfit(model);
    }
}