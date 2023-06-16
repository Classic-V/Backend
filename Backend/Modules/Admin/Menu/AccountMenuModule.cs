using Backend.Controllers.Event.Interface;
using Backend.Services.Account.Interface;
using Backend.Services.Ban.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Player.Client;
using Newtonsoft.Json;

namespace Backend.Modules.Admin.Menu;

public class AccountMenuModule : Module<AccountMenuModule>
{
    private IAccountService _accountService;
    private IBanService _banService;
    public AccountMenuModule(IEventController eventController, IAccountService accountService, IBanService banService) : base("AdminPlayerMenu")
    {
        _accountService = accountService;
        _banService = banService;

        #region Menu Events
            eventController.OnClient("Server:Admin:OpenAccounts", OpenAccounts);
            eventController.OnClient<int>("Server:Admin:OpenPlayerMenu", OpenPlayerMenu);
            eventController.OnClient<int>("Server:Admin:OpenAccountMenu", OpenAccountMenu);
        #endregion
        
        #region Player Events
            eventController.OnClient<string>("Server:Admin:Kick:Reason", Kick);
            eventController.OnClient<string>("Server:Admin:SendMessage", SendMessage);
        #endregion

        #region Account Events
            eventController.OnClient<string>("Server:Admin:ChangeName:Name", ChangeName);
            eventController.OnClient<int>("Server:Admin:BanPlayer:OpenInput", BanAccountMenu);
            eventController.OnClient<int, string, bool, int>("Server:Admin:BanPlayer:Input", BanAccount);
        #endregion
    }

    #region Menu Events
        private async void OpenAccountFilter(ClPlayer player, string eventKey)
        {
            if (player.DbModel.AdminRank < AdminRank.SUPPORTER) return;

            var items = new List<ClientNativeMenuItem>();

            items.Add(new ClientNativeMenuItem("Name:") { Type = NativeMenuItemType.INPUT });
            items.Add(new ClientNativeMenuItem("Nur online") { Type = NativeMenuItemType.CHECKBOX });
            items.Add(new ClientNativeMenuItem("Suchen") { CallbackEvent = "Server:Admin:OpenFilterdAccounts" });
            
            await player.ShowNativeMenu(true, new ClientNativeMenu("Admin Menu", items, "Account suchen"));
        }
        private async void OpenAccounts(ClPlayer player, string eventKey)
        {
            if (player.DbModel.AdminRank < AdminRank.SUPPORTER) return;

            var items = new List<ClientNativeMenuItem>();

            foreach (var account in _accountService.Accounts)
            {
                items.Add(new ClientNativeMenuItem(account.Name + " (" + account.Id + ")") { CallbackEvent = "Server:Admin:OpenPlayerMenu", CallbackArgs = new object[] {account.Id}, Type = NativeMenuItemType.SUBMENU});
            }
        
            await player.ShowNativeMenu(true, new ClientNativeMenu("Admin Menu", items, "Account suchen"));
        }
        private async void OpenPlayerMenu(ClPlayer player, string eventKey, int accountId)
        {
            if (player.DbModel.AdminRank < AdminRank.SUPPORTER) return;
        
            var targetAccount = _accountService.GetAccount(accountId);
            if (targetAccount == null)
            {
                await player.Notify("Administation", "Dieser Account existiert nicht!", NotificationType.ERROR);
                return;   
            }
            
            var targetPlayer = ClPlayer.LoggedIn.Find(x => x.DbModel.Id == accountId);
            if (targetPlayer == null)
            {
                OpenAccountMenu(player, eventKey, accountId);
                return;
            }
        
            var items = new List<ClientNativeMenuItem>();
            
            items.Add(new ClientNativeMenuItem("Teleportieren") { CallbackEvent = "Server:Admin:Teleport", CallbackArgs = new object[] {targetAccount.Name}, Type = NativeMenuItemType.SELECTOR, Value = 0, Data = new object[]
            {
                new ClientNativeMenuSelectionData("Zum Spieler", "to"),
                new ClientNativeMenuSelectionData("Zu mir", "here"),
            }});
            
            items.Add(new ClientNativeMenuItem("Spieler respawn") { CallbackEvent = "Server:Command:respawn", CallbackArgs = new object[] {targetAccount.Name}});
            if (!targetAccount.Alive) items.Add(new ClientNativeMenuItem("Spieler wiederbeleben") { CallbackEvent = "Server:Command:revive", CallbackArgs = new object[] { targetAccount.Name } });
            else items.Add(new ClientNativeMenuItem("Spieler heilen") { CallbackEvent = "Server:Command:heal", CallbackArgs = new object[] {targetAccount.Name}});
            items.Add(new ClientNativeMenuItem("Spieler freezen") { CallbackEvent = "Server:Command:freeze", CallbackArgs = new object[] {targetAccount.Name}});
            items.Add(new ClientNativeMenuItem("Spieler kicken") { CallbackEvent = "Server:Admin:Kick:Reason", CallbackArgs = new object[] {targetAccount.Name}, Type = NativeMenuItemType.SUBMENU});
            items.Add(new ClientNativeMenuItem("Spieler ban") { CallbackEvent = "Server:Admin:BanPlayer:OpenInput", CallbackArgs = new object[] {targetAccount.Id}, Type = NativeMenuItemType.SUBMENU });
            items.Add(new ClientNativeMenuItem("Nachricht senden") { CallbackEvent = "Server:Admin:SendMessage", CallbackArgs = new object[] {targetAccount.Name}, Type = NativeMenuItemType.SUBMENU });
            
            items.Add(new ClientNativeMenuItem("Account verwalten") { CallbackEvent = "Server:Admin:OpenAccountMenu", CallbackArgs = new object[] {targetAccount.Id}, Type = NativeMenuItemType.SUBMENU});
            
            await player.ShowNativeMenu(true, new ClientNativeMenu("Admin Menu", items, targetAccount.Name + " (" + targetAccount.Id + ")"));
        }
        private async void OpenAccountMenu(ClPlayer player, string eventKey, int accountId)
        {
            if (player.DbModel.AdminRank < AdminRank.SUPPORTER) return;
        
            var target = _accountService.GetAccount(accountId);
            if (target == null)
            {
                await player.Notify("Administation", "Dieser Account existiert nicht!", NotificationType.ERROR);
                return;   
            }
        
            var items = new List<ClientNativeMenuItem>();
        
            if (player.DbModel.AdminRank >= AdminRank.MODERATOR)
            {
                items.Add(new ClientNativeMenuItem("Id zurücksetzen") { CallbackEvent = "Server:Admin:ResetId", CallbackArgs = new object[] {target.Id}, Type = NativeMenuItemType.SELECTOR, Value = 0, Data = new object[]
                {
                    new ClientNativeMenuSelectionData("Discord", "dc"),
                    new ClientNativeMenuSelectionData("Hardware", "hwid"),
                    new ClientNativeMenuSelectionData("Socialclub", "social"),
                }});
            
                items.Add(new ClientNativeMenuItem("Gegenstand geben") { CallbackEvent = "Server:Admin:OpenGiveMenu", Type = NativeMenuItemType.SUBMENU, CallbackArgs = new object[] { accountId }});
                items.Add(new ClientNativeMenuItem("Namen ändern") { CallbackEvent = "Server:Admin:ChangeName:Name", CallbackArgs = new object[] {target.Name}, Type = NativeMenuItemType.SUBMENU });
                items.Add(new ClientNativeMenuItem("Account ban") { CallbackEvent = "Server:Admin:BanPlayer:OpenInput", CallbackArgs = new object[] {target.Id}, Type = NativeMenuItemType.SUBMENU });
                
                // items.Add(new ClientNativeMenuItem("Inventar zurücksetzen") { CallbackEvent = "Server:Command:clearinv", CallbackArgs = new object[] {target.Name}});
                // items.Add(new ClientNativeMenuItem("Waffen zurücksetzen") { CallbackEvent = "Server:Command:clearwep", CallbackArgs = new object[] {target.Name}});
            }
        
            await player.ShowNativeMenu(true, new ClientNativeMenu("Admin Menu", items, target.Name + " (" + target.Id + ")"));
        }

    #endregion

    #region Player Events
        private async void Kick(ClPlayer player, string eventKey, string targetName)
        {
            if (player.DbModel.AdminRank < AdminRank.SUPPORTER) return;
            
            var items = new List<ClientNativeMenuItem>();

            items.Add(new ClientNativeMenuItem("Grund:") { Type = NativeMenuItemType.INPUT });
            items.Add(new ClientNativeMenuItem("Spieler kicken") { CallbackEvent = "Server:Command:kick", CallbackArgs = new object[] { targetName }});
            
            await player.ShowNativeMenu(true, new ClientNativeMenu("Admin Menu", items, targetName + " kicken"));
        }

        private async void SendMessage(ClPlayer player, string eventKey, string targetName)
        {
            if (player.DbModel.AdminRank < AdminRank.SUPPORTER) return;
            
            var items = new List<ClientNativeMenuItem>();

            items.Add(new ClientNativeMenuItem("Nachricht:") { Type = NativeMenuItemType.INPUT });
            items.Add(new ClientNativeMenuItem("Nachricht senden") { CallbackEvent = "Server:Command:pm", CallbackArgs = new object[] { targetName }});
            
            await player.ShowNativeMenu(true, new ClientNativeMenu("Admin Menu", items, "Nachricht senden"));
        }
    #endregion

    #region Account Events
        private async void ChangeName(ClPlayer player, string eventKey, string targetName)
        {
            if (player.DbModel.AdminRank < AdminRank.MODERATOR) return;
            
            var items = new List<ClientNativeMenuItem>();

            items.Add(new ClientNativeMenuItem("Name:") { Type = NativeMenuItemType.INPUT });
            items.Add(new ClientNativeMenuItem("Umbenennen") { CallbackEvent = "Server:Command:setname", CallbackArgs = new object[] { targetName }});
            
            await player.ShowNativeMenu(true, new ClientNativeMenu("Admin Menu", items, targetName + " umbenennen"));
        }

        private async void BanAccountMenu(ClPlayer player, string eventKey, int accountId)
        {
            if (player.DbModel.AdminRank < AdminRank.MODERATOR) return;

            var targetAccount = _accountService.Accounts.FirstOrDefault(x => x.Id == accountId);
            if (targetAccount == null) return;
            
            var items = new List<ClientNativeMenuItem>();

            items.Add(new ClientNativeMenuItem("Grund:") { Type = NativeMenuItemType.INPUT });
            items.Add(new ClientNativeMenuItem("Permanent") { Type = NativeMenuItemType.CHECKBOX });
            items.Add(new ClientNativeMenuItem("Dauer (Tage)") { Type = NativeMenuItemType.INPUT, Data = "number", Value = 1 });
            items.Add(new ClientNativeMenuItem("Ban") { Type = NativeMenuItemType.BUTTON, Close = true, CallbackEvent = "Server:Admin:BanPlayer:Input", CallbackArgs = new object[] { accountId }});
            
            await player.ShowNativeMenu(true, new ClientNativeMenu("Admin Menu", items, targetAccount.Name + " ban")); 
        }

        private async void BanAccount(ClPlayer player, string eventKey, int accountId, string reason, bool permanent, int days)
        {
            if (player.DbModel.AdminRank < AdminRank.MODERATOR) return;

            var targetAccount = _accountService.Accounts.FirstOrDefault(x => x.Id == accountId);
            if (targetAccount == null) return;

            var banModel = new BanModel(accountId, reason, permanent, days);
            await _banService.AddBan(banModel);
            await player.Notify("Adminstration",
                "Du hast den Spieler " + targetAccount.Name + " gebannt. Die Ban Id ist: #" + banModel.Id,
                NotificationType.SUCCESS);

            var targetPlayer = ClPlayer.All.Find(x => x.DbModel != null && x.DbModel.Id == accountId);
            if (targetPlayer == null) return;
            
            targetPlayer.Kick("Du wurdest vom Gameserver gesperrt! Für weitere Informationen kannst du dich im Support melden. (Ban Id: #" + banModel.Id + ")");
        }
    #endregion
}