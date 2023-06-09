using AltV.Net.Data;
using Backend.Utils.Enums;
using Backend.Utils.Models.ClothesShop;

namespace Backend.Utils.Models.Database;

public class ClothesShopModel
{
    public int Id { get; set; }
    public PositionModel Position { get; set; }
    public ClothesShopType Type { get; set; }

    public ClothesShopModel() {}

    public ClothesShopModel(PositionModel position, ClothesShopType type)
    {
        Position = position;
        Type = type;
    }
}