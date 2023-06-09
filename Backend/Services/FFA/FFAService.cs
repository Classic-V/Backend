using AltV.Net.Events;
using Backend.Services.FFA.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;

namespace Backend.Services.FFA;

public class FFAService : IFFAService
{
    public List<FFAModel> FFA { get; private set; } = new();

    public FFAService()
    {
        using var ctx = new DBContext();

        FFA = new List<FFAModel>(ctx.FFA);
    }
    
    public Task<FFAModel?> GetFFA(int id) => Task.FromResult(FFA.FirstOrDefault(x => x.Id == id));
    public Task<FFAModel?> GetFFA(string name) => Task.FromResult(FFA.FirstOrDefault(x => x.Name == name));

    public Task Create(int maxplayers, string password, string name, uint firstWeapon, uint secondWeapon)
    {
        var ffa = new FFAModel()
        {
            MaxPlayers = maxplayers,
            Name = name,
            Password = password,
            Players = 0,
            Private = true,
            Radius = 50,
        };
        
        FFA.Add(ffa);

        return Task.CompletedTask;
    }

    public Task Close(int id)
    {
        var item = FFA.FirstOrDefault(x => x.Id == id);
        if(item != null)
            FFA.Remove(item);

        return Task.CompletedTask;
    }
}