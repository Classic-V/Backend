using Backend.Controllers.Event.Interface;
using Backend.Controllers.Jail.Interface;
using Backend.Services.FederalLicense.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Player.Client;

namespace Backend.Modules.Chat.Public.Player
{
    public class FederalCommandModule : CommandModule<FederalCommandModule>
    {
        private readonly IJailController _jailController;
        private readonly IFederalLicenseService _federalLicenseService;

        private DateTime _lastLocated = DateTime.Now.AddMinutes(-3);
        private DateTime _lastFindTarget = DateTime.Now.AddMinutes(-3);

        public FederalCommandModule(IEventController eventController, IJailController jailController, IFederalLicenseService federalLicenseService) : base("FederalCommand", AdminRank.PLAYER)
        {
            _jailController = jailController;
            _federalLicenseService = federalLicenseService;

            eventController.OnClient("Server:Command:wanteds", WantedsMenu);
            eventController.OnClient<string>("Server:Command:find", Find);
            eventController.OnClient<int>("Server:Federal:locate", Locate);
        }

        private async void WantedsMenu(ClPlayer player, string eventKey)
        {
            if (player.DbModel == null! || player.DbModel.Team != 2) return;

            var wantedPlayersItems = new List<ClientNativeMenuItem>();

            ClPlayer.All.ForEach(async target =>
            {
                if (target.DbModel == null! || !await _jailController.PlayerHasJailCrimes(target.DbModel.Id)) return;

                //wantedPlayersItems.Add(new ClientNativeMenuItem($"{target.DbModel.Name.Replace('_', ' ')} ({await _jailController.GetPlayerJailDuration(target.DbModel.Id)} Hafteinheiten)", false, "Server:Federal:Locate", target.DbModel.Id));
                wantedPlayersItems.Add(new ClientNativeMenuItem($"{target.DbModel.Name.Replace('_', ' ')} ({await _jailController.GetPlayerJailDuration(target.DbModel.Id)} Hafteinheiten)") { CallbackEvent = "Server:Federal:Locate", CallbackArgs = new object[] {target.DbModel.Id}});
            });

            var menu = new ClientNativeMenu("Wanteds Liste", wantedPlayersItems);
            await player.ShowNativeMenu(true, menu);
        }

        public async void Find(ClPlayer player, string eventKey, string target)
        {
            if (player.DbModel == null || player.DbModel.Team != 2) return;

            var licenses = await _federalLicenseService.GetLicense(player.DbModel.Id);
            if (licenses == null || !licenses.GpsFindLicense) return;

            ClPlayer findTarget = ClPlayer.All.FirstOrDefault(x => x.DbModel.Name.ToLower() == target.ToLower())!;
            if (findTarget == null || findTarget.DbModel == null) return;

            var locationPositionX = new Random().Next((int)findTarget.Position.X - 50, (int)findTarget.Position.X + 50);
            var locationPositionY = new Random().Next((int)findTarget.Position.Y - 50, (int)findTarget.Position.Y + 50);

            if (DateTime.Now < _lastFindTarget.AddSeconds(60))
            {
                await player.Notify("Ortungssystem", "Es wurde bereits in der letzten Minute jemand geortet.", NotificationType.INFO);
                return;
            }

            if (findTarget.DbModel.Team > 5)
            {
                if (!await _jailController.PlayerHasJailCrimes(findTarget.DbModel.Id))
                {
                    await player.Notify("Ortungssystem", "Der Spieler hat keine Offene Akte um geortet zu werden", NotificationType.ERROR);
                    return;
                }

                player.Emit("Client:PlayerModule:SetWaypoint", locationPositionX, locationPositionY);
                await player.Notify("Ortungssystem", $"Du hast {findTarget.DbModel.Name} geortet!", NotificationType.INFO);
                return;
            }

            _lastFindTarget = DateTime.Now;
            player.Emit("Client:PlayerModule:SetWaypoint", locationPositionX, locationPositionY);
            await player.Notify("Ortungssystem", $"Du hast {findTarget.DbModel.Name} geortet!", NotificationType.INFO);
        }

        private async void Locate(ClPlayer player, string eventKey, int id)
        {
            if (player.DbModel == null! || id <= 0) return;

            ClPlayer wanted = ClPlayer.All.FirstOrDefault(x => x.DbModel.Id == id)!;
            if (wanted == null!) return;

            if (player.DbModel.TeamRank >= 1 && player.DbModel.TeamRank <= 12)
            {
                if (DateTime.Now < _lastLocated.AddMinutes(1))
                {
                    await player.Notify("Ortungssystem", $"Es wurde bereits jemand in den letzten 1 Minuten geortet!", NotificationType.INFO);
                    return;
                }
            }

            var locationPositionX = new Random().Next((int)wanted.Position.X - 100, (int)wanted.Position.X + 100);
            var locationPositionY = new Random().Next((int)wanted.Position.Y - 100, (int)wanted.Position.Y + 100);

            _lastLocated = DateTime.Now;
            player.Emit("Client:PlayerModule:SetWaypoint", locationPositionX, locationPositionY);
            await player.Notify("Ortungssystem", $"Du hast {wanted.DbModel.Name} geortet (100m Umkreis)!", NotificationType.INFO);
        }
    }
}
