using AltV.Net.Data;
using AltV.Net;
using AltV.Net.Elements.Entities;
using Backend.Controllers.Event.Interface;
using Backend.Controllers.PizzaDeliveryJob.Interface;
using Backend.Data;
using Backend.Services.PizzaDeliveryJob.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Inventory;
using Backend.Utils.Models.Player.Client;
using Backend.Utils.Models.Vehicle;
using Backend.Utils.Streamer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net.Enums;
using Backend.Services.House.Interface;
using Backend.Controllers.House.Interface;
using System.Numerics;

namespace Backend.Modules.Jobs
{
    public class PizzaDeliveryJobModule : Module<PizzaDeliveryJobModule>, IEventColshape
    {
        private readonly IPizzaDeliveryJobController _pizzaDeliveryJobController;
        private readonly IPizzaDeliveryJobService _pizzaDeliveryJobService;
        private readonly IHouseController _houseController;

        public PizzaDeliveryJobModule(IEventController eventController, IPizzaDeliveryJobController pizzaDeliveryJobController, IPizzaDeliveryJobService pizzaDeliveryJobService, IHouseController houseController) : base("PizzaDeliveryJob")
        {
            _pizzaDeliveryJobController = pizzaDeliveryJobController;
            _pizzaDeliveryJobService = pizzaDeliveryJobService;
            _houseController = houseController;

            eventController.OnClient("Server:PizzaDeliveryJob:Open", Open);
            eventController.OnClient("Server:PizzaDeliveryJob:Join", Join);
            eventController.OnClient("Server:PizzaDeliveryJob:Quit", Quit);
            eventController.OnClient("Server:PizzaDeliveryJob:Return", Return);
        }

        private async void Open(ClPlayer player, string eventKey)
        {
            if (player.DbModel == null!) return;

            var shape = player.CurrentShape!;
            if (shape == null || shape.ShapeType != ColshapeType.PIZZA_DELIVERY_JOB) return;

            var items = new List<ClientNativeMenuItem>();
            if (!PlayerIsInJob(player.DbModel.Id)) items.Add(new ClientNativeMenuItem("Job anfangen", true, "Server:PizzaDeliveryJob:Join"));
            else items.Add(new ClientNativeMenuItem("Job beenden", true, "Server:PizzaDeliveryJob:Quit"));

            var menu = new ClientNativeMenu("Pizza Lieferant Job", items);
            await player.ShowNativeMenu(true, menu);
        }

        private async void Join(ClPlayer player, string eventKey)
        {
            if (player.DbModel == null!) return;
            if (PlayerIsInJob(player.DbModel.Id)) return;

            if (_pizzaDeliveryJobService.PizzaDeliveryJobs.Where(x => x.RouteOwner == 0).ToList().Count <= 0)
            {
                await player.Notify("Piza Lieferant", "Es gibt aktuell keine freien Routen.", NotificationType.ERROR);
                return;
            }

            var jobId = new Random().Next(1, _pizzaDeliveryJobService.PizzaDeliveryJobs.Where(x => x.RouteOwner == 0).ToList().Count);
            var job = await _pizzaDeliveryJobController.GetPizzaDeliveryJob(jobId);
            if (job == null!) return;

            var vehicleData = new Utils.Models.Database.VehicleModel
            {
                Type = VehicleType.TEMPORARY,
                Fuel = 100,
                Garage = -1,
                GloveBox = new InventoryModel(0, 0, InventoryType.VEHICLE_GLOVEBOX),
                Id = player.DbModel.Id,
                Owner = player.DbModel.Id,
                Position = job.VehiclePosition,
                InfoModelId = -1,
                Note = "Pizza",
                Parked = false,
                Plate = "Pizza",
                Trunk = new InventoryModel(0, 0, InventoryType.VEHICLE_TRUNK),
                Tuning = new TuningModel()
            };

            job.Vehicle = new ClVehicle(Alt.Core, (uint)AltV.Net.Enums.VehicleModel.Faggio3, job.VehiclePosition.Position,
                new Rotation(0, 0, job.VehiclePosition.H), vehicleData, 100);
            if (job.Vehicle.DbModel == null) return;
            job.Vehicle.DbModel = vehicleData;
            job.Vehicle.DbModel.Owner = player.DbModel.Id;
            job.RouteOwner = player.DbModel.Id;

            await player.Notify("Pizza Lieferant", "Du hast den Pizza Lieferanten Job angefangen!", NotificationType.SUCCESS);

            var firstHouse = job.Deliveries.FirstOrDefault(x => !x.IsDelivered);
            if (firstHouse == null) return;

            var house = await _houseController.GetHouse(firstHouse.HouseId);
            if (house == null) return;

            player.Emit("Client:PlayerModule:SetWaypoint", house.Position.X, house.Position.Y);

            job.RouteMarker = new Marker(1, new Position(house.Position.X, house.Position.Y, house.Position.Z - 1), new Rgba(0, 0, 255, 255), 100, 0);
            MarkerStreamer.AddMarker(job.RouteMarker);
        }

        private async void Quit(ClPlayer player, string eventKey)
        {
            if (player.DbModel == null!) return;
            if (!PlayerIsInJob(player.DbModel.Id)) return;

            var currentJob = _pizzaDeliveryJobService.PizzaDeliveryJobs.FirstOrDefault(x => x.RouteOwner == player.DbModel.Id);
            if (currentJob == null!) return;

            if (currentJob.Vehicle == null || currentJob.Vehicle.IsDestroyed || currentJob.Vehicle.HealthData == null)
            {
                await player.Notify("Pizza Lieferant", "Dein Pizza Roller wurde zerstört. Du erhälst kein Geld.", NotificationType.ERROR);
                currentJob.ResetRoute();
                return;
            }

            if (currentJob.Vehicle.Position.Distance(PizzaDeliveryJobModel.StartPosition) > 30f)
            {
                await player.Notify("Pizza Lieferant", "Dein Pizza Roller ist zu weit entfernt um den Job zu beenden.", NotificationType.ERROR);
                player.Emit("Client:PlayerModule:SetWaypoint", currentJob.Vehicle.Position.X, currentJob.Vehicle.Position.Y);
                return;
            }

            double price;

            if (!currentJob.ReachedMaxHouses())
            {
                var percentage = (double)currentJob.ReturnedHouses() / currentJob.MaxDeliveries;
                price = percentage * currentJob.MaxPrice;
            }
            else price = currentJob.MaxPrice;

            currentJob.ResetRoute();
            await player.Notify("Pizza Lieferant", $"Du hast den Job beendet und {price}$ erhalten",
                NotificationType.SUCCESS);
            await player.AddMoney((int)Math.Round(price, MidpointRounding.ToEven));
        }

        private async void Return(ClPlayer player, string eventKey)
        {
            if (player.DbModel == null!) return;

            var shape = player.CurrentShape;
            if (shape == null) return;

            var job = _pizzaDeliveryJobService.PizzaDeliveryJobs.FirstOrDefault(x => x.RouteOwner == player.DbModel.Id);
            if (job == null!) return;

            if (job.ReachedMaxHouses())
            {
                await player.Notify("Pizza Lieferant", "Du hast bereits alle Pizzen abgeliefert!", NotificationType.INFO);
                player.Emit("Client:PlayerModule:SetWaypoint", PizzaDeliveryJobModel.StartPosition.X, PizzaDeliveryJobModel.StartPosition.Y);
                return;
            }

            var houseId = shape.Id;

            var currentHouse = job.Deliveries.FirstOrDefault(x => x.HouseId == houseId);
            if (currentHouse == null) return;
            if (currentHouse.IsDelivered)
            {
                await player.Notify("Pizza Lieferant", "Die Pizza wurde hier bereits abgeben!", NotificationType.INFO);
                return;
            }

            currentHouse.IsDelivered = true;

            if (job.ReachedMaxHouses())
            {
                await player.Notify("Pizza Lieferant",
                    "Du hast alle Pizzen abgegeben! Bring den Roller zurück und erhalte dein Geld.", NotificationType.INFO);
                player.Emit("Client:PlayerModule:SetWaypoint", PizzaDeliveryJobModel.StartPosition.X, PizzaDeliveryJobModel.StartPosition.Y);
                MarkerStreamer.RemoveMarker(job.RouteMarker.Id);
                return;
            }

            var nextHouse = job.Deliveries.FirstOrDefault(x => !x.IsDelivered);
            if (nextHouse == null) return;

            var house = await _houseController.GetHouse(nextHouse.HouseId);
            if (house == null) return;

            await player.Notify("Pizza Lieferant", "Pizza erfolgreich abgegeben!", NotificationType.SUCCESS);
            player.Emit("Client:PlayerModule:SetWaypoint", house.Position.X, house.Position.Y);

            MarkerStreamer.RemoveMarker(job.RouteMarker.Id);

            job.RouteMarker = new Marker(1, new Position(house.Position.X, house.Position.Y, house.Position.Z - 1), new Rgba(0, 0, 255, 255), 100, 0);
            MarkerStreamer.AddMarker(job.RouteMarker);
        }

        public async Task OnColshape(ClShape shape, IEntity entity, bool entered)
        {
            if (!entered || shape == null || entity.Type != BaseObjectType.Player || shape.ShapeType != ColshapeType.PIZZA_DELIVERY_JOB) return;

            ClPlayer player = (ClPlayer)entity;
            if (player == null) return;

            player.SetInteraction(Interactions.KEY_E, entered ? Interactions.E_PIZZA_JOB_JOIN : Interactions.NONE);
        }

        private bool PlayerIsInJob(int playerId)
        {
            return _pizzaDeliveryJobService.PizzaDeliveryJobs.FirstOrDefault(x => x.RouteOwner == playerId) != null;
        }
    }
}
