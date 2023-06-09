using System.ComponentModel.DataAnnotations.Schema;
using AltV.Net.Data;
using Backend.Utils.Enums;

namespace Backend.Utils.Models.Database;

public class TrainingStationModel
{
    public int Id { get; set; }
    public PositionModel Position { get; set; }
    public TrainingType Type { get; set; }

    [NotMapped] public bool Occupied { get; set; } = false;
    [NotMapped] public int OccupantId { get; set; } = 0;

    public TrainingStationModel(PositionModel position, TrainingType type)
    {
        Position = position;
        Type = type;
    }
}