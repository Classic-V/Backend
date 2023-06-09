using AltV.Net.Elements.Entities;
using Backend.Controllers.FFA.Interface;
using Backend.Services.FFA.Interface;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;

namespace Backend.Controllers.FFA;

public class FFAController : IFFAController
{
    private readonly IFFAService _ffaService;

    public FFAController(IFFAService ffaService)
    {
        _ffaService = ffaService;
    }

    public async void LoadFFAs()
    {
        await Task.Delay(5000);
        
        _ffaService.FFA.ForEach(LoadFFA);
    }
    
    private async void LoadFFA(FFAModel model)
    {
        var ffaModel = _ffaService.GetFFA(model.Id).Result;
    }

    public Task Join(ClPlayer player, int id, string password = "")
    {
        var ffa = _ffaService.GetFFA(id).Result;
        if (ffa == null) return Task.CompletedTask;

        if (password != ffa.Password) return Task.CompletedTask;
        
        var pos = ffa.Spawns[new Random().Next(ffa.Spawns.Count)];

        player.IsInInterior = true;
        player.OutsideInteriorPosition = player.Position;

        player.RemoveAllWeapons();
        
        player.SetDimension(ffa.Id);
        player.SetPosition(pos.Position);
        player.SetHealth(200, 100);
        
        player.IsInFFA = true;
        player.FFAId = id;
        
        return Task.CompletedTask; 
    }

    public Task Leave(ClPlayer player)
    {
        if (!player.IsInFFA) return Task.CompletedTask;
        
        player.IsInFFA = false;
        player.FFAId = -1;

        
        player.SetPosition(player.OutsideInteriorPosition);
        player.IsInInterior = false;
        player.SetDimension(0);

        player.RemoveAllWeapons(); 

        player.ApplyWeapons();
        
        return Task.CompletedTask;
    }

    public Task Create(ClPlayer player, int maxplayers,  string password, string name, uint firstWeapon, uint secondWeapon)
    {
        _ffaService.Create(maxplayers, password, name, firstWeapon, secondWeapon);

        var ffa = _ffaService.GetFFA(name); 
        
        Join(player, ffa.Id, password); 
        return Task.CompletedTask; 
    }

    public Task PlayerDeath(ClPlayer player, IEntity? killer)
    {
        var ffa = _ffaService.GetFFA(player.FFAId).Result;

        player.SetPosition(ffa.Spawns[new Random().Next(ffa.Spawns.Count)].Position);
        player.SetHealth(200, 100);
        
        player.StopAnimation();
        
        return Task.CompletedTask; 
    }
}