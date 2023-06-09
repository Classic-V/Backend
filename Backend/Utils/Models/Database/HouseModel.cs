using System.ComponentModel.DataAnnotations.Schema;
using AltV.Net.Data;
using Backend.Utils.Enums;
using Backend.Utils.Models.House;
using Backend.Utils.Models.Inventory;

namespace Backend.Utils.Models.Database;

public class HouseModel
{
    public static PositionModel LowPosition { get; } = new(265.9912f, -1007.2747f, -101.02f, 0);
    public static PositionModel MediumPosition { get; } = new(346.6813f, -1012.6286f, -99.2f, 0);
    public static PositionModel HighPosition { get; } = new(-774.0349f, 342.0296f, 196.6862f, 0);

    public static Position LowWardrobePosition { get; } = new(260.5f, -1004.004f, -99.02f);
    public static Position MediumWardrobePosition { get; } = new(350.62f, -993.7f, -99.2f);
    public static Position HighWardrobePosition { get; } = new(-763.23f, 329.7f, 199.5f);

    public static Position LowInventoryPosition { get; } = new(264.8f, -996.5f, -99.02f);
    public static Position MediumInventoryPosition { get; } = new(343.11f, -1002.04f, -99.2f);
    public static Position HighInventoryPosition { get; } = new(-764.57f, 330.48f, 196.07f);

    public int Id { get; set; }
    public int Owner { get; set; }
    public HouseType Type { get; set; }
    public Position Position { get; set; }
    public List<RentMemberData> RentMembers { get; set; }
    public int Price { get; set; }
    public InventoryModel Inventory { get; set; } = new (400, 25, InventoryType.STORAGE);
    public int WardrobeId { get; set; }

    [NotMapped] public bool Locked { get; set; } = true;

    public HouseModel() {}

    public HouseModel(int owner, HouseType type, Position position, List<RentMemberData> rentMembers, int price, InventoryModel inventory, int wardrobeId)
    {
        Owner = owner;
        Type = type;
        Position = position;
        RentMembers = rentMembers;
        Price = price;
        Inventory = inventory;
        WardrobeId = wardrobeId;
    }

    public bool CanAccess(int id)
    {
        return (Owner == id) || RentMembers.Find(x => x.Id == id) != null;
    }
}