using AltV.Net.Data;
using Backend.Controllers.Wardrobe.Interface;
using Backend.Services.House.Interface;
using Backend.Utils;
using Backend.Utils.Enums;
using Backend.Utils.Models.Database;

namespace Backend.Services.House;

public class HouseService : IHouseService
{
    private readonly IWardrobeController _wardrobeController;

    public List<HouseModel> Houses { get; }

    public HouseService(IWardrobeController wardrobeController)
    {
        var ctx = new DBContext();
        Houses = new List<HouseModel>(ctx.House);

        _wardrobeController = wardrobeController;
    }

    public Task<HouseModel?> GetHouse(int id)
    {
        return Task.FromResult(Houses.FirstOrDefault(x => x.Id == id));
    }

    public async Task AddHouse(HouseModel model)
    {
        Houses.Add(model);

        var ctx = new DBContext();
        ctx.House.Add(model);
        await ctx.SaveChangesAsync();

        var wardrobe = -1;

        switch (model.Type)
        {
            case HouseType.LOW:
                wardrobe =
                    await _wardrobeController.CreateWardrobe(new WardrobeModel(3f, HouseModel.LowWardrobePosition, WardrobeType.NORMAL,
                        model.Id + 50));
                break;
            case HouseType.MEDIUM:
                wardrobe =
                    await _wardrobeController.CreateWardrobe(new WardrobeModel(3f, HouseModel.MediumWardrobePosition, WardrobeType.NORMAL,
                        model.Id + 50));
                break;
            case HouseType.HIGH:
                wardrobe =
                    await _wardrobeController.CreateWardrobe(new WardrobeModel(3f, HouseModel.HighWardrobePosition, WardrobeType.NORMAL,
                        model.Id + 50));
                break;
            default: break;
        }

        model.WardrobeId = wardrobe;

        await UpdateHouse(model);
    }

    public async Task UpdateHouse(HouseModel model)
    {
        var ctx = new DBContext();
        ctx.House.Update(model);
        await ctx.SaveChangesAsync();
    }
}