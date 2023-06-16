using Backend.Controllers.Event.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Player.Client;

namespace Backend.Modules.Admin.Menu;

public class ServerMenuModule : Module<ServerMenuModule>
{
    public ServerMenuModule(IEventController eventController) : base("ServerMenu")
    {
        eventController.OnClient("Server:Admin:OpenServerMenu", OpenServerMenu);
        eventController.OnClient("Server:Admin:OpenAnnouncementMenu", OpenAnnouncementMenu);
    }

    private async void OpenServerMenu(ClPlayer player, string eventKey)
    {
        if (player.DbModel.AdminRank < AdminRank.SUPPORTER) return;

        var items = new List<ClientNativeMenuItem>();

        if (player.DbModel.AdminRank >= AdminRank.ADMINISTRATOR)
        {
            items.Add(new ClientNativeMenuItem("Announcement schreiben") { CallbackEvent = "Server:Admin:OpenAnnouncementMenu", Type = NativeMenuItemType.SUBMENU });
            
            items.Add(new ClientNativeMenuItem("Wetter setzen") { Type = NativeMenuItemType.SELECTOR, CallbackEvent = "Server:Command:setweather", Value = 0, Data = new object[]
            {
                new ClientNativeMenuSelectionData("Extra Sunny", 0),
                new ClientNativeMenuSelectionData("Clear", 1),
                new ClientNativeMenuSelectionData("Clouds", 2),
                new ClientNativeMenuSelectionData("Smog", 3),
                new ClientNativeMenuSelectionData("Foggy", 4),
                new ClientNativeMenuSelectionData("Overcast", 5),
                new ClientNativeMenuSelectionData("Rain", 6),
                new ClientNativeMenuSelectionData("Thunder", 7),
                new ClientNativeMenuSelectionData("Light rain", 8),
                new ClientNativeMenuSelectionData("Smoggy light rain", 9),
                new ClientNativeMenuSelectionData("Very light snow", 10),
                new ClientNativeMenuSelectionData("Windy light snow", 11),
                new ClientNativeMenuSelectionData("Light snow", 12),
                new ClientNativeMenuSelectionData("Christmas", 13),
                new ClientNativeMenuSelectionData("Halloween", 14),
            }});
        }

        await player.ShowNativeMenu(true, new ClientNativeMenu("Admin Menu", items, "Server verwaltung"));
    }

    private async void OpenAnnouncementMenu(ClPlayer player, string eventKey)
    {
        if (player.DbModel.AdminRank < AdminRank.ADMINISTRATOR) return;

        var items = new List<ClientNativeMenuItem>();
        
        items.Add(new ClientNativeMenuItem("Text:") { Type = NativeMenuItemType.INPUT, Data = "text" });
        items.Add(new ClientNativeMenuItem("Absenden") { CallbackEvent = "Server:CommandFulltext:announce"});

        await player.ShowNativeMenu(true, new ClientNativeMenu("Admin Menu", items, "Ankündigung schreiben"));
    }
}