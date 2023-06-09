using Backend.Utils.Models.Entities;

namespace Backend.Controllers.Login.Interface;

public interface ILoginController
{
    Task Login(ClPlayer player, string password);
    Task OnConnect(ClPlayer player);
}