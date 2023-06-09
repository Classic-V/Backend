using AltV.Net.Async;
using Backend.Controllers.Event.Interface;
using Backend.Controllers.Login.Interface;
using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;

namespace Backend.Modules.Login;

public class LoginModule : Module<LoginModule>, IEventPlayerConnect, IEventPlayerDisconnect
{
    private readonly ILoginController _loginController;
    
    public LoginModule(IEventController eventController, ILoginController loginController) : base("Login")
    {
        _loginController = loginController;

		eventController.OnClient<string>("Server:Login:Submit", OnPlayerLogin);
    }

    private async void OnPlayerLogin(ClPlayer player, string eventKey, string password)
    {
        await _loginController.Login(player, password);
    }

    public async Task OnPlayerConnect(ClPlayer player, string reason)
    {
        await _loginController.OnConnect(player);
    }

    public Task OnPlayerDisconnect(ClPlayer player, string reason)
    {
        ClPlayer.All.Remove(player);
        return Task.CompletedTask;
    }
}