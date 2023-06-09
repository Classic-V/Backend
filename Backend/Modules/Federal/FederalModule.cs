using AltV.Net.Elements.Entities;
using Backend.Controllers.Event.Interface;
using Backend.Data;
using Backend.Services.FederalLicense.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Input;
using Backend.Utils.Models.Player.Client;
using Newtonsoft.Json;

namespace Backend.Modules.Federal
{
    public class FederalModule : Module<FederalModule>, IEventColshape
    {
        private readonly IFederalLicenseService _federalLicenseService;

        public FederalModule(IEventController eventController, IFederalLicenseService federalLicenseService) : base("Federal")
        {
            _federalLicenseService = federalLicenseService;

            eventController.OnClient("Server:Federal:OpenLicense", OpenLicense);
            eventController.OnClient<int, string>("Server:Federal:GiveLicenseBox", GiveLicenseBox);
            eventController.OnClient<int, int>("Server:Federal:GiveLicenseConfirm", GiveLicenseConfirm);
            eventController.OnClient<int, int>("Server:Federal:GiveLicense", GiveLicense);
        }

        private async void OpenLicense(ClPlayer player, string eventKey)
        {
            if (player.DbModel == null || player.DbModel.Team != 2 || player.DbModel.TeamRank < 11) return;

            await player.ShowComponent("Input", true,
            JsonConvert.SerializeObject(new InputDataModel("Agenten Name", "Gebe den Namen des Agenten an!",
                InputType.TEXT, "Server:Federal:GiveLicenseBox", player.DbModel.Id)));
        }

        private async void GiveLicenseBox(ClPlayer player, string eventKey, int senderId, string targetName)
        {
            if (senderId <= 0 || player.DbModel == null || player.DbModel.Team != 2 || player.DbModel.TeamRank < 11) return;

            ClPlayer target = ClPlayer.All.FirstOrDefault(x => x.DbModel.Name.ToLower() == targetName.ToLower())!;
            if (target == null || target.DbModel == null || target.DbModel.Team != 2) return;

            var targetLicenses = await _federalLicenseService.GetLicense(target.DbModel.Id);
            if (targetLicenses == null) targetLicenses = new FederalLicenseModel(target.DbModel.Id, false, false, false);

            var gpsLicense = targetLicenses!.GpsFindLicense ? "✓" : "✘";
            var historyLicense = targetLicenses!.PhoneHistoryLicense ? "✓" : "✘";
            var sadLicense = targetLicenses!.SadLicense ? "✓" : "✘";

            var items = new List<ClientNativeMenuItem>()
            {
                new ($"Ortungs Lizenz ({gpsLicense})", true, "Server:Federal:GiveLicenseConfirm", target.DbModel.Id, FederalLicenseTypes.GPS),
                new ($"Telefon Verlauf ({historyLicense})", true, "Server:Federal:GiveLicenseConfirm", target.DbModel.Id, FederalLicenseTypes.PHONE_HISTORY),
                new ($"SAD Dienst ({sadLicense})", true, "Server:Federal:GiveLicenseConfirm", target.DbModel.Id, FederalLicenseTypes.SAD),
            };

            var menu = new ClientNativeMenu("IT Lizenzen", items);
            await player.ShowNativeMenu(true, menu);
        }

        private async void GiveLicenseConfirm(ClPlayer player, string eventKey, int targetId, int licenseType)
        {
            if (targetId <= 0 || licenseType < 0 || player.DbModel == null || player.DbModel.Team != 2 || player.DbModel.TeamRank < 11) return;

            ClPlayer target = ClPlayer.All.FirstOrDefault(x => x.DbModel.Id == targetId)!;
            if (target == null || target.DbModel == null || target.DbModel.Team != 2) return;

            var targetLicenses = await _federalLicenseService.GetLicense(targetId);
            if (targetLicenses == null) targetLicenses = new FederalLicenseModel(target.DbModel.Id, false, false, false);

            var title = (FederalLicenseTypes)licenseType == FederalLicenseTypes.GPS ? "GPS Lizenz" : (FederalLicenseTypes)licenseType == FederalLicenseTypes.SAD ? "SAD Dienst Lizenz" : "Telefon Verlauf Lizenz";
            var message = "";

            if (targetLicenses.GpsFindLicense && (FederalLicenseTypes)licenseType == FederalLicenseTypes.GPS) message = "entziehen";
            else if (!targetLicenses.GpsFindLicense && (FederalLicenseTypes)licenseType == FederalLicenseTypes.GPS) message = "geben";

            if (targetLicenses.PhoneHistoryLicense && (FederalLicenseTypes)licenseType == FederalLicenseTypes.PHONE_HISTORY) message = "entziehen";
            else if (!targetLicenses.PhoneHistoryLicense && (FederalLicenseTypes)licenseType == FederalLicenseTypes.PHONE_HISTORY) message = "geben";

            if (targetLicenses.SadLicense && (FederalLicenseTypes)licenseType == FederalLicenseTypes.SAD) message = "entziehen";
            else if (!targetLicenses.SadLicense && (FederalLicenseTypes)licenseType == FederalLicenseTypes.SAD) message = "geben";

            await player.ShowComponent("Input", true,
            JsonConvert.SerializeObject(new InputDataModel($"{title}", $"Sicher das du {target.DbModel.Name} diese Lizenz {message} möchtest?",
                InputType.CONFIRM, "Server:Federal:GiveLicense", targetId, licenseType)));
        }

        private async void GiveLicense(ClPlayer player, string eventKey, int targetId, int licenseType)
        {
            if (targetId <= 0 || licenseType < 0 || player.DbModel == null || player.DbModel.Team != 2 || player.DbModel.TeamRank < 11) return;

            ClPlayer target = ClPlayer.All.FirstOrDefault(x => x.DbModel.Id == targetId)!;
            if (target == null || target.DbModel == null || target.DbModel.Team != 2) return;

            var licenseTypes = (FederalLicenseTypes)licenseType;

            var targetLicenses = await _federalLicenseService.GetLicense(targetId);
            if (targetLicenses == null)
            {
                var license = new FederalLicenseModel(target.DbModel.Id, false, false, false);
                await _federalLicenseService.AddLicense(license);
                targetLicenses = license;
            }

            if (licenseTypes == FederalLicenseTypes.GPS) targetLicenses!.GpsFindLicense = !targetLicenses.GpsFindLicense;
            else if (licenseTypes == FederalLicenseTypes.PHONE_HISTORY) targetLicenses!.PhoneHistoryLicense = !targetLicenses.PhoneHistoryLicense;
            else if (licenseTypes == FederalLicenseTypes.SAD) targetLicenses!.SadLicense = !targetLicenses.SadLicense;

            await _federalLicenseService.UpdateLicense(targetLicenses!);
            await player.Notify("IT Lizenzen", $"Du hast {target.DbModel.Name} seine IT Lizenzen bearbeitet", NotificationType.SUCCESS);
        }

        public async Task OnColshape(ClShape shape, IEntity entity, bool entered)
        {
            if (shape.ShapeType != ColshapeType.FEDERAL_LICENSE || entity.Type != BaseObjectType.Player) return;

            ClPlayer player = (ClPlayer) entity;
            if (player == null! || player.DbModel == null! || player.DbModel.Team != 2 || player.DbModel.TeamRank < 11) return;

            player.SetInteraction(Interactions.KEY_E, entered ? Interactions.E_FEDERAL_LICENSE_OPEN : Interactions.NONE);
        }
    }
}
