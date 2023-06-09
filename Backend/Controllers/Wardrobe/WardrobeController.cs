using AltV.Net;
using AltV.Net.Data;
using Backend.Controllers.Wardrobe.Interface;
using Backend.Services.Wardrobe.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Streamer;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;

namespace Backend.Controllers.Wardrobe;

public class WardrobeController : IWardrobeController
{
    private readonly IWardrobeService _wardrobeService;

    public WardrobeController(IWardrobeService wardrobeService)
    {
        _wardrobeService = wardrobeService;

        _wardrobeService.Wardrobes.ForEach(LoadWardrobe);
    }

    public void LoadWardrobe(WardrobeModel model)
    {
        var colShape = (ClShape)Alt.CreateColShapeSphere(model.Position, model.Scale);
        colShape.Id = model.Id;
        colShape.ShapeType = ColshapeType.WARDROBE;
        colShape.Size = model.Scale;
        colShape.Dimension = model.Dimension;
    }

    public async Task<WardrobeModel?> GetWardrobe(int id)
    {
        return await _wardrobeService.GetWardrobe(id);
    }

    public async Task<int> CreateWardrobe(WardrobeModel model)
    {
        await _wardrobeService.AddWardrobe(model);

        return model.Id;
    }
}