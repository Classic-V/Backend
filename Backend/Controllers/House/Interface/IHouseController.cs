using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Shops;

namespace Backend.Controllers.House.Interface;

public interface IHouseController
{
    void LoadHouses(HouseModel model);
    Task<HouseModel?> GetHouse(int id);
    Task<int> CreateHouse(HouseModel model);
    Task<PositionModel?> GetHouseInsidePosition(int id);
    void BuyHouse(ClPlayer player, int houseId);
    void AddRenter(ClPlayer player, int houseId, string targetName, int price);
}