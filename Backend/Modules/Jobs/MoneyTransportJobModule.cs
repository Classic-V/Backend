using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Backend.Controllers.Event.Interface;
using Backend.Controllers.MoneyTransportJob.Interface;
using Backend.Data;
using Backend.Services.Bank.Interface;
using Backend.Services.MoneyTransportJob.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Interfaces.Events;
using System.Linq;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Inventory;
using Backend.Utils.Models.Player.Client;
using Backend.Utils.Streamer;

namespace Backend.Modules.Jobs
{
    public class MoneyTransportJobModule : Module<MoneyTransportJobModule>, IEventColshape
    {
        private readonly IMoneyTransportJobService _moneyTransportJobService;
        private readonly IMoneyTransportJobController _moneyTransportJobController;
        private readonly IBankService _bankService;

        public MoneyTransportJobModule(IEventController eventController, IMoneyTransportJobService moneyTransportJobService, IMoneyTransportJobController moneyTransportJobController, IBankService bankService) : base("MoneyTransportJob")
        {
            _moneyTransportJobService = moneyTransportJobService;
            _moneyTransportJobController = moneyTransportJobController;
            _bankService = bankService;

            eventController.OnClient("Server:MoneyTransportJob:Open", Open);
            eventController.OnClient("Server:MoneyTransportJob:Join", Join);
            eventController.OnClient("Server:MoneyTransportJob:Quit", Quit);
            eventController.OnClient("Server:MoneyTransportJob:Return", Return);
        }
        
        private async void Open(ClPlayer player, string eventKey)
        {
            if (player.DbModel == null) return;

            var items = new List<ClientNativeMenuItem>();
            if (!_moneyTransportJobController.PlayerIsInMoneyJob(player.DbModel.Id)) items.Add(new ClientNativeMenuItem("Job anfangen", true, "Server:MoneyTransportJob:Join"));
            else items.Add(new ClientNativeMenuItem("Job beenden", true, "Server:MoneyTransportJob:Quit"));

            var menu = new ClientNativeMenu("Geldtransporter Job", items);
            await player.ShowNativeMenu(true, menu);
        }

        private async void Join(ClPlayer player, string eventKey)
        {
            if (player.DbModel == null) return;

            var shape = player.CurrentShape;
            if (shape == null || shape.ShapeType != ColshapeType.MONEY_TRANSPORT_JOB) return;

            if (_moneyTransportJobService.MoneyTransportJobs.Where(x => x.RouteOwner == 0).ToList().Count <= 0)
            {
                await player.Notify("Geldtransporter", "Es gibt aktuell keine freien Routen!", NotificationType.ERROR);
                return;
            }

            var jobId = new Random().Next(1, _moneyTransportJobService.MoneyTransportJobs.Where(x => x.RouteOwner == 0).ToList().Count);
            var job = await _moneyTransportJobController.GetMoneyTransportJob(jobId);
            if (job == null) return;

            var firstBank = job.Ids.FirstOrDefault(x => !x.Returned);
            if (firstBank == null) return;

            var bank = _bankService.GetBankById(firstBank.BankId);
            if (bank == null) return;

            var vehicleData = new VehicleModel
            {
                Fuel = 100,
                Garage = 0,
                GloveBox = new InventoryModel(0, 0, InventoryType.VEHICLE_GLOVEBOX),
                InfoModelId = -1,
                Id = player.DbModel.Id,
                Note = "Geldtransport",
                Owner = player.DbModel.Id,
                Parked = false,
                Plate = "Geldtransporter",
                Position = job.VehiclePosition,
                Trunk = new InventoryModel(0, 0, InventoryType.VEHICLE_TRUNK),
                Tuning = new Utils.Models.Vehicle.TuningModel(),
                Type = VehicleType.TEMPORARY
            };

            job.Vehicle = new ClVehicle(Alt.Core, 0x6827CF72, job.VehiclePosition.Position, new Rotation(0, 0, job.VehiclePosition.H), vehicleData, 100);
            job.Vehicle.DbModel = vehicleData;
            job.Vehicle.DbModel.Owner = player.DbModel.Id;
            job.RouteOwner = player.DbModel.Id;

            await player.Notify("Geldtransporter", $"Du hast den Job als Geldtransporter angefangen!", NotificationType.SUCCESS);
            player.Emit("Client:PlayerModule:SetWaypoint", bank.Position.X, bank.Position.Y);

            job.RouteMarker = new Marker(1, new Position(bank.Position.X, bank.Position.Y, bank.Position.Z -1), new Rgba(0, 0, 255, 255), 100, 0);
            MarkerStreamer.AddMarker(job.RouteMarker);
        }

        private async void Quit(ClPlayer player, string eventKey)
        {
            if (player.DbModel == null) return;

            var shape = player.CurrentShape;
            if (shape == null || shape.ShapeType != ColshapeType.MONEY_TRANSPORT_JOB) return;

            if (!_moneyTransportJobController.PlayerIsInMoneyJob(player.DbModel.Id))
            {
                await player.Notify("Geldtransporter", "Du hast keine aktive Route!", NotificationType.ERROR);
                return;
            }

            var job = _moneyTransportJobService.MoneyTransportJobs.FirstOrDefault(x => x.RouteOwner == player.DbModel.Id);
            if (job  == null) return;

            if (job.Vehicle == null || job.Vehicle.IsDestroyed || job.Vehicle.HealthData == null)
            {
                await player.Notify("Geldtransporter", "Dein Geldtransporter wurde zerstört. Du erhälst kein Geld.", NotificationType.INFO);
                job.ResetRoute();
                return;
            }

            if (job.Vehicle.Position.Distance(MoneyTransportJobModel.StartPosition) > 30f)
            {
                await player.Notify("Geldtransporter", "Dein Fahrzeug ist zuweit entfernt um es zurück zu geben!", NotificationType.ERROR);
                return;
            }

            double price;

            if (!job.ReturnedAllNotes())
            {
                var percentage = (double)job.ReturnedNotes() / job.Ids.Count;
                price = percentage * job.MaxPrice;
            }
            else price = job.MaxPrice;

            job.ResetRoute();

            await player.Notify("Geldtransporter", $"Du hast den Job beendet und {price}$ erhalten",
                NotificationType.SUCCESS);
            await player.AddMoney((int)price);
            player.Emit("Client:PlayerModule:SetWaypoint", MoneyTransportJobModel.StartPosition.X, MoneyTransportJobModel.StartPosition.Y);
        }

        private async void Return(ClPlayer player, string eventKey)
        {
            if (player.DbModel == null) return;
            if (!_moneyTransportJobController.PlayerIsInMoneyJob(player.DbModel.Id)) return;

            var shape = player.CurrentShape;
            if (shape == null || shape.ShapeType != ColshapeType.BANK) return;

            var job = _moneyTransportJobService.MoneyTransportJobs.FirstOrDefault(x => x.RouteOwner == player.DbModel.Id);
            if (job == null) return;

            var bank = _bankService.GetBankById(shape.Id);
            if (bank == null) return;

            if (job.ReturnedAllNotes())
            {
                await player.Notify("Geldtransporter", "Du hast bereits alle Banknoten zurückgegeben! Gebe das Fahrzeug ab und erhalte dein Geld!", NotificationType.INFO);
                return;
            }

            var jobBank = job.Ids.FirstOrDefault(x => x.BankId == bank.Id);
            if (jobBank == null) return;
            if (jobBank.Returned)
            {
                await player.Notify("Geldtransporter", "Die Banknoten wurde hier bereits abgeben!", NotificationType.INFO);
                return;
            }

            jobBank.Returned = true;
            await player.Notify("Geldtransporter", "Du hast die Banknoten abgegeben!", NotificationType.SUCCESS);
            MarkerStreamer.RemoveMarker(job.RouteMarker.Id);

            if (!job.ReturnedAllNotes())
            {
                var nextBankId = job.Ids.FirstOrDefault(x => !x.Returned);
                if (nextBankId == null) return;

                var nextBank = _bankService.GetBankById(nextBankId.BankId);
                if (nextBank == null) return;

                player.Emit("Client:PlayerModule:SetWaypoint", nextBank.Position.X, nextBank.Position.Y);
                job.RouteMarker = new Marker(1, new Position(nextBank.Position.X, nextBank.Position.Y, nextBank.Position.Z - 1), new Rgba(0, 0, 255, 255), 100, 0);
                MarkerStreamer.AddMarker(job.RouteMarker);
                return;
            }

            await player.Notify("Geldtransporter", "Du hast alle Banknoten zurückgegeben! Gebe das Fahrzeug ab und erhalte dein Geld!", NotificationType.INFO);
            MarkerStreamer.RemoveMarker(job.RouteMarker.Id);
            player.Emit("Client:PlayerModule:SetWaypoint", MoneyTransportJobModel.StartPosition.X, MoneyTransportJobModel.StartPosition.Y);
        }

        public async Task OnColshape(ClShape shape, IEntity entity, bool entered)
        {
            if (entity.Type != BaseObjectType.Player || shape == null || shape.ShapeType != ColshapeType.MONEY_TRANSPORT_JOB) return;

            ClPlayer player = (ClPlayer)entity;
            if (player == null || player.DbModel == null) return;

            player.SetInteraction(Interactions.KEY_E, entered ? Interactions.E_MONEY_TRANSPORT_JOB_JOIN : Interactions.NONE);
        }
    }
}
