using AltV.Net;
using AltV.Net.Elements.Entities;
using Backend.Controllers.House.Interface;
using Backend.Modules.Inventory;
using Backend.Services.House.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.House;
using Backend.Utils.Models.Inventory;

namespace Backend.Controllers.House;

public class HouseController : IHouseController
{
    private readonly IHouseService _houseService;

    public HouseController(IHouseService houseService)
    {
        _houseService = houseService;

        _houseService.Houses.ForEach(LoadHouses);
    }

    public void LoadHouses(HouseModel model)
    {
        var colShape = (ClShape)Alt.CreateColShapeSphere(model.Position, 0.75f);
        colShape.Id = model.Id;
        colShape.Size = 0.75f;
        colShape.ShapeType = ColshapeType.HOUSE;
        colShape.Dimension = 0;
        colShape.SetData("Entry", true);

        switch (model.Type)
        {
            case HouseType.LOW:
                var lowShape = (ClShape)Alt.CreateColShapeSphere(HouseModel.LowPosition.Position, 0.75f);
                lowShape.Id = model.Id;
                lowShape.Size = 0.75f;
                lowShape.ShapeType = ColshapeType.HOUSE;
                lowShape.Dimension = model.Id + 50;
                lowShape.SetData("Entry", false);

                var lowInventoryShape = (ClShape)Alt.CreateColShapeSphere(HouseModel.LowInventoryPosition, 1.25f);
                lowInventoryShape.Id = model.Id;
                lowInventoryShape.Size = 1.25f;
                lowInventoryShape.ShapeType = ColshapeType.HOUSE_INVENTORY;
                lowInventoryShape.Inventory = model.Inventory;
                lowInventoryShape.Dimension = model.Id + 50;
                break;
            case HouseType.MEDIUM:
                var mediumShape = (ClShape)Alt.CreateColShapeSphere(HouseModel.MediumPosition.Position, 0.75f);
                mediumShape.Id = model.Id;
                mediumShape.Size = 0.75f;
                mediumShape.ShapeType = ColshapeType.HOUSE;
                mediumShape.Dimension = model.Id + 50;
                mediumShape.SetData("Entry", false);

                var mediumInventoryShape = (ClShape)Alt.CreateColShapeSphere(HouseModel.MediumInventoryPosition, 1.25f);
                mediumInventoryShape.Id = model.Id;
                mediumInventoryShape.Size = 1.25f;
                mediumInventoryShape.ShapeType = ColshapeType.HOUSE_INVENTORY;
                mediumInventoryShape.Inventory = model.Inventory;
                mediumInventoryShape.Dimension = model.Id + 50;
                break;
            case HouseType.HIGH:
                var highShape = (ClShape)Alt.CreateColShapeSphere(HouseModel.HighPosition.Position, 0.75f);
                highShape.Id = model.Id;
                highShape.Size = 0.75f;
                highShape.ShapeType = ColshapeType.HOUSE;
                highShape.Dimension = model.Id + 50;
                highShape.SetData("Entry", false);

                var highInventoryShape = (ClShape)Alt.CreateColShapeSphere(HouseModel.HighInventoryPosition, 1.25f);
                highInventoryShape.Id = model.Id;
                highInventoryShape.Size = 1.25f;
                highInventoryShape.ShapeType = ColshapeType.HOUSE_INVENTORY;
                highInventoryShape.Inventory = model.Inventory;
                highInventoryShape.Dimension = model.Id + 50;
                break;
            default: break;
        }

    }

    public async Task<HouseModel?> GetHouse(int id)
    {
        return await _houseService.GetHouse(id);
    }

    public async Task<int> CreateHouse(HouseModel model)
    {
        await _houseService.AddHouse(model);    

        return model.Id;
    }

    public async Task<PositionModel?> GetHouseInsidePosition(int id)
    {
        var houseModel = await _houseService.GetHouse(id);
        if (houseModel == null) return null;

        return houseModel.Type == HouseType.LOW ? HouseModel.LowPosition :
            houseModel.Type == HouseType.MEDIUM ? HouseModel.MediumPosition : HouseModel.HighPosition;
    }

    public async void BuyHouse(ClPlayer player, int houseId)
    {
        if (player == null! || player.DbModel == null! || houseId <= 0) return;

        var house = await _houseService.GetHouse(houseId);
        if (house == null) return;

        if (house.Owner != 0)
        {
            await player.Notify("Haus", $"Das Haus ist bereits vergeben!", NotificationType.ERROR);
            return;
        }

        if (_houseService.Houses.FirstOrDefault(x => x.Owner == player.DbModel.Id) != null) 
        {
            await player.Notify("Haus", $"Du besitzt bereits ein Haus!", NotificationType.ERROR);
            return;
        }

        if (player.DbModel.Money < house.Price)
        {
            await player.Notify("Haus", $"Du hast nicht genügend Geld dabei!", NotificationType.ERROR);
            return;
        }

        house.Owner = player.DbModel.Id;
        house.RentMembers = new List<RentMemberData>();

        await _houseService.UpdateHouse(house);
        await player.RemoveMoney(house.Price);
        await player.Notify("Haus", $"Du hast das Haus ({house.Id}) erfolgreich gekauft!", NotificationType.SUCCESS);
    }

    public async void AddRenter(ClPlayer player, int houseId, string targetName, int price)
    {
        if (targetName.Length <= 0 || houseId <= 0) return;
        ClPlayer target = ClPlayer.All.FirstOrDefault(x => x.DbModel.Name.Equals(targetName))!;
        if (target == null!) return;

        var house = await _houseService.GetHouse(houseId);
        if (house == null) return;

        if (house.CanAccess(houseId))
        {
            await player.Notify("Haus", "Du kannst niemanden einmieten als nicht Besitzer!", NotificationType.ERROR);
            return;
        }

        var alreadyRented = _houseService.Houses.Find(x => x.RentMembers.FirstOrDefault(x => x.Id == target.DbModel.Id) != null) != null;
        if (alreadyRented)
        {
            await player.Notify("Haus", "Der Spieler ist bereits eingemietet!", NotificationType.ERROR);
            return;
        }

        if (target.Position.Distance(player.Position) > 10f)
        {
            await player.Notify("Haus", "Der Spieler ist nicht in deiner Nähe!", NotificationType.ERROR);
            return;
        }

        house.RentMembers.Add(new RentMemberData(target.DbModel.Id, price));

        await _houseService.UpdateHouse(house);
        await player.Notify("Haus", $"Du hast den {target.DbModel.Name} eingemietet!", NotificationType.SUCCESS);
        await target.Notify("Haus", "Du wurdest erfolgreich eingemietet.", NotificationType.SUCCESS);

        var itemModel = InventoryModule.GetItemBase(2023);
        if (itemModel == null) return;

        player.DbModel.Inventory.RemoveItem(itemModel, 1);
    }
}