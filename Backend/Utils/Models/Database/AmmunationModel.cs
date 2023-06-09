using AltV.Net.Data;
using Backend.Utils.Models.Ammunation;
using Backend.Utils.Models.Shops;

namespace Backend.Utils.Models.Database;

public class AmmunationModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Position Position { get; set; }
    public PositionModel PedPosition { get; set; } = new();
    public List<WeaponShopItemModel> Items { get; set; } = new();

    public AmmunationModel(string name, Position position, PositionModel pedPosition, List<WeaponShopItemModel> items)
    {
        Name = name;
        Position = position;
        PedPosition = pedPosition;
        Items = items;
    }
}