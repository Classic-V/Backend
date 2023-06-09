using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Utils.Models.Database;

public class FFAModel
{
    public int Id { get; set; }
    public string Name { get; set; } 
    public float Radius { get; set; }
    public int MaxPlayers { get; set; }
    public List<PositionModel> Spawns { get; set; }
    public uint FirstWeapon { get; set; }
    public uint SecondWeapon { get; set; }
    [NotMapped]
    public int Players { get; set; }
    [NotMapped]
    public bool Private { get; set; }

    [NotMapped] 
    public string Password { get; set; } = ""; 

}