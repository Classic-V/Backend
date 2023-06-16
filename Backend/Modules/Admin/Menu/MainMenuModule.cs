using Backend.Controllers.Event.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Player.Client;

namespace Backend.Modules.Admin.Menu;

public class MainMenuModule : Module<MainMenuModule>
{
    public MainMenuModule(IEventController eventController) : base("AdminMainMenu")
    {
        eventController.OnClient("Server:Admin:OpenMenu", OpenMenu);
    }
		
    public async void OpenMenu(ClPlayer player, string eventKey)
    {
        if (player.DbModel.AdminRank < AdminRank.SUPPORTER) return;

        var items = new List<ClientNativeMenuItem>();
			
        items.Add(new ClientNativeMenuItem("Accounts") { CallbackEvent = "Server:Admin:OpenAccounts", Type = NativeMenuItemType.SUBMENU });
        items.Add(new ClientNativeMenuItem("Bans") { CallbackEvent = "Server:Admin:OpenBansMenu", Type = NativeMenuItemType.SUBMENU });
        
        if (player.DbModel.AdminRank >= AdminRank.ADMINISTRATOR)
        {
            items.Add(new ClientNativeMenuItem("Server Verwaltung") { CallbackEvent = "Server:Admin:OpenServerMenu", Type = NativeMenuItemType.SUBMENU });
        }
        
        if (DevMenuModule.Devs.Find(x => x == player.Name) != null)
        {
            items.Add(new ClientNativeMenuItem("Dev Menu") { CallbackEvent = "Server:Dev:OpenMenu", Type = NativeMenuItemType.SUBMENU });
        }
        
        items.Add(new ClientNativeMenuItem("Aduty") { CallbackEvent = "Server:Command:aduty" });

        await player.ShowNativeMenu(true, new ClientNativeMenu("Admin Menu", items));
    }
}