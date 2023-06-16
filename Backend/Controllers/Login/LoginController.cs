using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Enums;
using Backend.Controllers.Login.Interface;
using Backend.Modules.Inventory;
using Backend.Utils;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Inventory;
using Backend.Utils.Models.Player;
using Backend.Utils.Streamer;
using Newtonsoft.Json;
using Backend.Utils.Models.Inventory.Database;
using Backend.Services.Account.Interface;
using Backend.Utils.Models.Player.Client;
using Backend.Modules.World;
using System.Text.RegularExpressions;
using AltV.Net;
using Backend.Controllers.Ban.Interface;
using Backend.Modules.Admin.Menu;
using Backend.Utils.Enums;
using Backend.Services.Door.Interface;

namespace Backend.Controllers.Login;

public class LoginController : ILoginController
{
    private readonly IAccountService _accountService;
    private readonly IDoorService _doorService;
    private readonly IBanController _banController;

    public LoginController(IAccountService accountService, IDoorService doorService, IBanController banController)
    {
		_accountService = accountService;
        _doorService = doorService;
        _banController = banController;
    }

    public async Task Login(ClPlayer player, string password)
    {
        var account = _accountService.GetAccount(player.Name);

		if (account == null)
        {
            player.Kick("Ein Fehler ist aufgetreten!");
            return;
        }

        if (account.Password.Length > 0)
        {
            if (!BCrypt.Net.BCrypt.Verify(password, account.Password))
            {
                await player.Notify("Information", "Das angegebene Passwort stimmt nicht überein!", NotificationType.ERROR);
                return;
            }
        }
        else
        {
            account.Password = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
        }

        if(account.AdminRank < AdminRank.SUPPORTER)
        {
            player.Kick("Du wurdest gekicked! Grund: Der Server ist aktuell nur für Teammitglieder zugänglich!");
            return;
        }

        // Todo: add unban date
        if(_banController.IsAccountBanned(account.Id).Result)
        {
            var ban = _banController.GetActiveBan(account.Id).Result;
            player.Kick("Du bist aktuell auf dem Gameserver gesperrt! Für weitere Informationen kannst du dich im Support melden. (Ban Id: #" + ban.Id + ")");
            return;
        }

        player.DbModel = account;

        if (!account.Customization.FinishedCreation)
		{
            await player.SetGodmode(false);
			await player.ShowComponent("Login", false);
            await player.SetDimension(player.Id);
            await player.SetPosition(new Position(400.94507f, -1004.9011f, -99.01465f));
			await player.ShowComponent("Creator", true, JsonConvert.SerializeObject(account.Customization));
            player.IsInInterior = true;
            player.OutsideInteriorPosition = account.LastPos.Position;
            player.Emit("Client:DoorModule:Init", JsonConvert.SerializeObject(_doorService.Doors));
            return;
        }

        await player.Load();
		await player.ShowComponent("Login", false);
        player.Emit("Client:DoorModule:Init", JsonConvert.SerializeObject(_doorService.Doors));
        player.SetSyncedMetaData("NAME", account.Name);
	}

    public async Task OnConnect(ClPlayer player)    
	{
		var regex = new Regex("([a-zA-Z]+)_([a-zA-Z]+)");
		var regex2 = new Regex("([a-zA-Z]+)_([a-zA-Z]+)-([a-zA-Z]+)");

        if(!regex.Match(player.Name).Success && !regex2.Match(player.Name).Success)
        {
            player.Kick("Du wurdest gekicked! Grund: Dein Name muss dem Format Vorname_Nachname(-Doppelname) entsprechen!");
            return;
        }

		var account = _accountService.GetAccount(player.Name);
		if (account == null) account = await Register(player);
        if (account == null) return;

        if(ClPlayer.All.Where(x => x.Name == player.Name).Count() > 1)
        {
            player.Kick("Du wurdest gekicked! Grund: Dieser Account ist bereits eingeloggt!");
            return;
        }

        if (DevMenuModule.Devs.Find(x => x == player.Name) == null)
        {
            if (account.SocialClub > 0 && player.SocialClubId != account.SocialClub)
            {
                player.Kick("Du wurdest gekicked! Grund: Es existiert bereits ein Account mit diesem Namen.");
                return;
            }
        
            if ((account.HardwareId > 0 && player.HardwareIdHash != account.HardwareId) || (account.HardwareIdEx > 0 && player.HardwareIdExHash != account.HardwareIdEx))
            {
                player.Kick("Du wurdest gekicked! Grund: HWID Mismatch.");
                return;
            }

            if(player.DiscordId == 0)
            {
                player.Kick("Du wurdest gekicked! Grund: Discord muss geöffnet sein!");
                return;
            }

            if(account.DiscordId != 0 && player.DiscordId != account.DiscordId)
            {
                player.Kick("Du wurdest gekicked! Grund: Discord Mismatch.");
                return;
            }
        }

        account.SocialClub = player.SocialClubId;
        account.HardwareId = player.HardwareIdHash;
        account.HardwareIdEx = player.HardwareIdExHash;
        account.DiscordId = player.DiscordId;

		await player.SetDimension(player.Id);
		await player.SetModel(0x9C9EFFD8);
        player.Spawn(new Position(0, 0, 72));
        await player.SetPosition(new Position(0, 0, 72));
        await player.SetGodmode(true);
        await player.ShowComponent("Login", true, player.Name);
	}

    private async Task<AccountModel> Register(ClPlayer player)
    {
		var multiAccount = _accountService.Accounts.FirstOrDefault(x => (x.SocialClub != 0 && x.SocialClub == player.SocialClubId) || (x.DiscordId != 0 && player.DiscordId != 0 && x.DiscordId == player.DiscordId));
		if (multiAccount != null)
		{
			player.Kick($"Du wurdest gekicked! Grund: Du hast bereits einen Account! ({multiAccount.Name})");
            return null;
        }

        var hwidAcc = _accountService.Accounts.FirstOrDefault(x => (x.HardwareId != 0 && x.HardwareId == player.HardwareIdHash) || (x.HardwareIdEx != 0 && x.HardwareIdEx == player.HardwareIdExHash));
		if (hwidAcc != null)
        {
            // Todo: add log
            Console.WriteLine($"[MULTIACCOUNT] {player.Name}: HWID");
        }
        
        var account = new AccountModel
        {
            Name = player.Name,
            SocialClub = player.SocialClubId,
            HardwareId = player.HardwareIdHash,
            HardwareIdEx = player.HardwareIdExHash,
            DiscordId = player.DiscordId
        };

        await _accountService.AddAccount(account);
        return account;
    }
}