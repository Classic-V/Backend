using AltV.Net.Elements.Entities;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;

namespace Backend.Controllers.FFA.Interface;

public interface IFFAController
{
    Task Join (ClPlayer player, int id, string password = "");
    Task Leave(ClPlayer player);
    Task Create(ClPlayer player, int maxplayers, string password, string name, uint firstWeapon, uint secondWeapon);
    Task PlayerDeath(ClPlayer player, IEntity? killer);
}