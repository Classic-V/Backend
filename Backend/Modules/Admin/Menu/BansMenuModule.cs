using System.Buffers.Text;
using Backend.Controllers.Event.Interface;
using Backend.Services.Account.Interface;
using Backend.Services.Ban.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Player.Client;

namespace Backend.Modules.Admin.Menu;

public class BansMenuModule : Module<BansMenuModule>
{
    private readonly IBanService _banService;
    private readonly IAccountService _accountService;
    public BansMenuModule(IEventController eventController, IBanService banService, IAccountService accountService) : base("BansMenu")
    {
        _accountService = accountService;
        _banService = banService;
        eventController.OnClient("Server:Admin:OpenBansMenu", OpenBansMenu);
        eventController.OnClient<int>("Server:Admin:OpenBan", OpenBanMenu);
    }

    public async void OpenBansMenu(ClPlayer player, string eventKey)
    {
        if (player.DbModel.AdminRank < AdminRank.SUPPORTER) return;

        var items = new List<ClientNativeMenuItem>();
        foreach (var banServiceBan in _banService.Bans)
        {
            items.Add(new ClientNativeMenuItem($"#{banServiceBan.Id}") { CallbackEvent = "Server:Admin:OpenBan", CallbackArgs = new object[] { banServiceBan.Id }, Type = NativeMenuItemType.SUBMENU});
        }

        await player.ShowNativeMenu(true, new ClientNativeMenu("Admin Menu", items, "Bans"));
    }
    
    
    public async void OpenBanMenu(ClPlayer player, string eventKey, int banId)
    {
        if (player.DbModel.AdminRank < AdminRank.SUPPORTER) return;

        var ban = _banService.Bans.Find(x => banId == x.Id);
        if (ban == null)
        {
            await player.Notify("Administration", "Die Ban Id konnte nicht gefunden werden.", NotificationType.ERROR);
            return;
        }

        var account = _accountService.GetAccount(ban.AccountId);
        if (account == null)
        {
            await player.Notify("Administration", "Der Account zu diesem Ban exestiert nicht mehr.", NotificationType.ERROR);
            return;
        }
        
        var items = new List<ClientNativeMenuItem>();
        
        items.Add(new ClientNativeMenuItem("Gebannter Spieler: " + account.Name) {Type = NativeMenuItemType.INFO});
        // items.Add(new ClientNativeMenuItem("Gebannt von: " + ban.BannedFrom) {Type = NativeMenuItemType.INFO});
        items.Add(new ClientNativeMenuItem("Gebannt für: " + ban.Reason) {Type = NativeMenuItemType.INFO});
        if (ban.Permanent) items.Add(new ClientNativeMenuItem("Gebannt bis: " + "Permanent") {Type = NativeMenuItemType.INFO});
        else items.Add(new ClientNativeMenuItem("Gebannt bis: " + ban.BannedTill.ToString("dd.MM.yyyy hh:mm")) {Type = NativeMenuItemType.INFO});
        items.Add(new ClientNativeMenuItem("Aktiv") {Type = NativeMenuItemType.CHECKBOX, Value = ban.Active, Data = "Server:Admin:ChangeBanState", CallbackArgs = new object[] {ban.Id}});

        await player.ShowNativeMenu(true, new ClientNativeMenu("Admin Menu", items, "Ban #" + ban.Id));
    }
}