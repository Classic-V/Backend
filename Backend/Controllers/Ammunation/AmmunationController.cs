using AltV.Net;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Backend.Controllers.Ammunation.Interface;
using Backend.Services.Ammunation.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Ammunation;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Streamer;

namespace Backend.Controllers.Ammunation;

public class AmmunationController : IAmmunationController
{
    private readonly IAmmunationService _ammunationService;

    public AmmunationController(IAmmunationService ammunationService)
    {
        _ammunationService = ammunationService;

        _ammunationService.Ammunations.ForEach(LoadAmmunation);
    }

    public void LoadAmmunation(AmmunationModel model)
    {
        var blip = Alt.CreateBlip(BlipType.Destination, model.Position);
        blip.Dimension = 0;
        blip.Sprite = 52;
        blip.Name = "Ammunation";
        blip.ShortRange = true;
        blip.Color = 2;

        var colShape = (ClShape)Alt.CreateColShapeSphere(model.Position, 1f);
        colShape.Id = model.Id;
        colShape.ShapeType = ColshapeType.AMMUNATION;
        colShape.Size = 1f;
        colShape.Dimension = 0;
        
        PedStreamer.AddObject(new Ped(0x18CE57D0, model.PedPosition.X,
            model.PedPosition.Y, model.PedPosition.Z, model.PedPosition.H, true, true, 0));
    }

    public async Task<AmmunationModel?> GetAmmunation(int id)
    {
        return await _ammunationService.GetAmmunation(id);
    }

    public async Task<WeaponShopItemModel?> GetWeaponItemData(int shopId, int id)
    {
        var shop = await _ammunationService.GetAmmunation(shopId);
        if (shop == null) return  null;

        return shop.Items.FirstOrDefault(x => x.ItemId == id);
    }

    public async Task<int> CreateAmmunation(AmmunationModel model)
    {
        var shop = await _ammunationService.AddAmmunation(model);
        if (shop == null!)
        {
            return -1;
        }

        return shop.Id;
    }
}