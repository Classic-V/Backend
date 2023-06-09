using Backend.Utils.Models.Database;

namespace Backend.Services.House.Interface;

public interface IHouseService
{
    List<HouseModel> Houses { get; }
    Task<HouseModel?> GetHouse(int id);
    Task AddHouse(HouseModel model);
    Task UpdateHouse(HouseModel model);
}