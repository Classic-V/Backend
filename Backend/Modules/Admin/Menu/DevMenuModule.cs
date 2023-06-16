using Backend.Controllers.Event.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Player.Client;

namespace Backend.Modules.Admin.Menu;

public class DevMenuModule : Module<DevMenuModule>
{
    public static List<String> Devs = new List<string>()
    {
        "Brian_McKenzie"
    };
    public DevMenuModule(IEventController eventController) : base("AdminMainMenu")
    {
        eventController.OnClient("Server:Dev:OpenMenu", OpenMenu);
    }
		
    public void OpenMenu(ClPlayer player, string eventKey)
    {
        if (Devs.Find(x => x == player.Name) == null) return;

        var items = new List<ClientNativeMenuItem>();
			
        items.Add(new ClientNativeMenuItem("Secret Aduty") { CallbackEvent = "Server:Command:aduty", CallbackArgs = new object[] { true }});
        items.Add(new ClientNativeMenuItem("Datenbank speichern") { CallbackEvent = "Server:Command:savedb" });
        
        player.ShowNativeMenu(true, new ClientNativeMenu("Dev Menu", items));
    }
}