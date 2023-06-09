using AltV.Net.Enums;
using Backend.Controllers.Event.Interface;
using Backend.Controllers.FFA.Interface;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Streamer;

namespace Backend.Modules.FFA;

public class FFAModule : Module<FFAModule>
{
    private readonly IFFAController _ffaController;
    
    public FFAModule(IEventController eventController, IFFAController ffaController): base("FFAModule")
    {
        _ffaController = ffaController;
        eventController.OnClient<int, string>("Server:FFA:Join", Join);
        eventController.OnClient("Server::FFA::Leave", Leave);
        eventController.OnClient<int, string, string, uint, uint>("Server::FFA::Create", Create);

        PedStreamer.AddObject(new Ped(0x61D201B3, 0, 0, 0, 2, true, true, 0));
    }

    public async void Join(ClPlayer player, string eventKey, int id, string password)
    {
        if (player.DbModel == null) return;
        await _ffaController.Join(player, id, password); 
    }

    public async void Leave(ClPlayer player, string eventKey)
    {
        if (!player.IsInFFA || player.DbModel == null) return;
        await _ffaController.Leave(player);
    }

    public async void Create(ClPlayer player, string eventKey, int maxplayers,  string password, string name, uint firstWeapon, uint secondWeapon)
    {
        if (player.IsInFFA || player.DbModel == null) return;
        await _ffaController.Create(player, maxplayers, password, name, firstWeapon, secondWeapon); 
    }
}