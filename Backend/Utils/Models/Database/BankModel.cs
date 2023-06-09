using AltV.Net.Data;
using Backend.Utils.Enums;

namespace Backend.Utils.Models.Database;

public class BankModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Position Position { get; set; }
    public BankType Type { get; set; }

    public BankModel() {}

    public BankModel(string name, Position position, BankType type)
    {
        Name = name;
        Position = position;
        Type = type;
    }
}