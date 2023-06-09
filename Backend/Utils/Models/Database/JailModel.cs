using AltV.Net.Data;

namespace Backend.Utils.Models.Database;

public class JailModel
{
    public int Id { get; set; }
    public float Scale { get; set; }
    public Position Position { get; set; }
    public List<Position> ImprisonPoint { get; set; }
    public Position ReleasePoint { get; set; }

    public JailModel() {}

    public JailModel(float scale, Position position, List<Position> imprisonPoint, Position releasePoint)
    {
        Scale = scale;
        Position = position;
        ImprisonPoint = imprisonPoint;
        ReleasePoint = releasePoint;
    }
}