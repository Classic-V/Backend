using Backend.Controllers.House.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Input;
using Newtonsoft.Json;

namespace Backend.Utils.Models.Inventory.Items;

public class Rentpaper : ItemBase
{
    private readonly IHouseController _houseController;

    public override int Id => 2023;

    public Rentpaper(IHouseController houseController) : base("Mietvertrag", 0.1f, 1)
    {
        _houseController = houseController;
    }

    public override async Task<bool> Use(ClPlayer player, int slot, int amount)
    {
        var shape = player.CurrentShape;
        if (shape == null || shape.ShapeType != ColshapeType.HOUSE) return false;

        shape.GetData("Entry", out bool result);
        if (!result)
        {
            await player.Notify("Mietvertrag", "Du musst am Eingang von deinem Haus stehen.", NotificationType.ERROR);
            return false;
        }

        var id = shape.Id;
        var house = await _houseController.GetHouse(id);
        if (house == null) return false;

        if (house.Owner != player.DbModel.Id)
        {
            await player.Notify("Mietvertrag", "Du musst Eigentümer dieses Hauses sein.", NotificationType.ERROR);
            return false;
        }

        await player.ShowComponent("Input", true,
            JsonConvert.SerializeObject(new InputDataModel("Mieter Hinzufügen", "Gebe den Namen des Mieters an.",
                InputType.TEXT, "Server:House:Rent", house.Id)));

        return true;
    }
}