using AltV.Net.Data;
using Backend.Utils.Enums;

namespace Backend.Utils.Models.Database;

public class WardrobeModel
{
    public int Id { get; set; }
    public float Scale { get; set; }
    public Position Position { get; set; }
    public WardrobeType Type { get; set; }
    public int Dimension { get; set; }

    public WardrobeModel()
    {
    }

    public WardrobeModel(float scale, Position position, WardrobeType type, int dimension)
    {
        Scale = scale;
        Position = position;
        Type = type;
        Dimension = dimension;
    }
}