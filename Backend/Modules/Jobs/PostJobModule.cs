using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Backend.Controllers.Event.Interface;
using Backend.Controllers.PostJob.Interface;
using Backend.Data;
using Backend.Utils.Enums;
using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Inventory;
using Backend.Utils.Models.Player.Client;
using Backend.Utils.Models.Vehicle;
using Discord.Rest;
using System.Diagnostics;
using Backend.Services.PostJob.Interface;
using VehicleModel = AltV.Net.Enums.VehicleModel;
using System.Numerics;
using Backend.Controllers.House.Interface;
using Backend.Utils.Streamer;
using System.Threading.Tasks.Dataflow;

namespace Backend.Modules.Jobs;

public class PostJobModule : Module<PostJobModule>, IEventColshape
{
    private readonly IPostJobController _postJobController;
    private readonly IHouseController _houseController;

    private readonly IPostJobService _postJobService;

    public PostJobModule(IPostJobController postJobController, IHouseController houseController, IPostJobService postJobService, IEventController eventController) : base("PostJob")
    {
        _postJobController = postJobController;
        _houseController = houseController;
        _postJobService = postJobService;

        eventController.OnClient("Server:PostJob:Open", Open);
        eventController.OnClient("Server:PostJob:Join", Join);
        eventController.OnClient("Server:PostJob:Return", Return);
    }

    private async void Open(ClPlayer player, string eventKey)
    {
        if (player.DbModel == null!) return;

        var shape = player.CurrentShape;
        if (shape == null! || shape.ShapeType != ColshapeType.POST_JOB_START) return;
        
        var items = new List<ClientNativeMenuItem>();
        if (!PlayerIsInJob(player.DbModel.Id)) items.Add(new("Job anfangen") { Close = true, CallbackEvent = "Server:PostJob:Join" });
        else items.Add(new("Job beenden") { Close = true, CallbackEvent = "Server:PostJob:Join" });

        var menu = new ClientNativeMenu("Post Job", items);
        await player.ShowNativeMenu(true, menu);
    }

    private async void Join(ClPlayer player, string eventKey)
    {
        if (player.DbModel == null!) return;

        if (PlayerIsInJob(player.DbModel.Id))
        {
            var currentJob = _postJobService.PostJobs.FirstOrDefault(x => x.RouteOwner == player.DbModel.Id);
            if (currentJob == null!) return;

            if (currentJob.Vehicle == null || currentJob.Vehicle.IsDestroyed || currentJob.Vehicle.HealthData == null)
            {
                await player.Notify("Post Job", "Dein Post Wagen wurde zerstört. Du erhälst kein Geld.", NotificationType.ERROR);
                currentJob.ResetRoute();
                return;
            }

            if (currentJob.Vehicle.Position.Distance(PostJobModel.StartPosition) > 30f)
            {
                await player.Notify("Post Job", "Dein Post Wagen ist zu weit entfernt um den Job zu beenden.", NotificationType.ERROR);
                player.Emit("Client:PlayerModule:SetWaypoint", currentJob.Vehicle.Position.X, currentJob.Vehicle.Position.Y);
                return;
            }

            double price;

            if (!currentJob.ReachedMaxHouses())
            {
                var percentage = (double) currentJob.ReturnedHouses() / currentJob.MaxHouses;
                price = percentage * currentJob.MaxPrice;
            }
            else price = currentJob.MaxPrice;

            currentJob.ResetRoute();
            await player.Notify("Post Job", $"Du hast den Job beendet und {price}$ erhalten",
                NotificationType.SUCCESS);
            await player.AddMoney((int) Math.Round(price, MidpointRounding.ToEven));

            return;

        }

        if (_postJobService.PostJobs.Where(x => x.RouteOwner == 0).ToList().Count <= 0)
        {
            await player.Notify("Post Job", "Es gibt aktuell keine freien Routen.", NotificationType.ERROR);
            return;
        }

        var jobId = new Random().Next(1, _postJobService.PostJobs.Where(x => x.RouteOwner == 0).ToList().Count);
        var job = await _postJobController.GetJob(jobId);
        if (job == null!) return;

        var vehicleData = new Utils.Models.Database.VehicleModel
        {
            Type = VehicleType.TEMPORARY,
            Fuel = 100,
            Garage = -1,
            GloveBox = new InventoryModel(0, 0, InventoryType.VEHICLE_GLOVEBOX),
            Id = player.DbModel.Id,
            Owner = player.DbModel.Id,
            Position = job.VehicleStart,
            InfoModelId = -1,
            Note = "POST",
            Parked = false,
            Plate = "POST",
            Trunk = new InventoryModel(0, 0, InventoryType.VEHICLE_TRUNK),
            Tuning = new TuningModel()
        };

        job.Vehicle = new ClVehicle(Alt.Core, (uint)VehicleModel.Boxville2, job.VehicleStart.Position,
            new Rotation(0, 0, job.VehicleStart.H), vehicleData, 100);
        if (job.Vehicle.DbModel == null) return;
        job.Vehicle.DbModel = vehicleData;
        job.Vehicle.DbModel.Owner = player.DbModel.Id;
        job.RouteOwner = player.DbModel.Id;

        await player.Notify("Post Job", "Du hast den Post Job angefangen!", NotificationType.SUCCESS);

        var firstHouse = job.ReturnPoints.FirstOrDefault(x => !x.Returned);
        if (firstHouse == null) return;

        var house = await _houseController.GetHouse(firstHouse.HouseId);
        if (house == null) return;

        player.Emit("Client:PlayerModule:SetWaypoint", house.Position.X, house.Position.Y);

        job.RouteMarker = new Marker(1, new Position(house.Position.X, house.Position.Y, house.Position.Z - 1), new Rgba(0, 0, 255, 255), 100, 0);
        MarkerStreamer.AddMarker(job.RouteMarker);
    }

    private async void Return(ClPlayer player, string eventKey)
    {
        if (player.DbModel == null!) return;

        var shape = player.CurrentShape;
        if (shape == null) return;

        var job = _postJobService.PostJobs.FirstOrDefault(x => x.RouteOwner == player.DbModel.Id);
        if (job == null!) return;

        if (job.ReachedMaxHouses())
        {
            await player.Notify("Post Job", "Du hast bereits alle Post-Pakete zurück gegeben!", NotificationType.INFO);
            player.Emit("Client:PlayerModule:SetWaypoint", PostJobModel.StartPosition.X, PostJobModel.StartPosition.Y);
            return;
        }

        var houseId = shape.Id;

        var currentHouse = job.ReturnPoints.FirstOrDefault(x => x.HouseId == houseId);
        if (currentHouse == null) return;
        if (currentHouse.Returned)
        {
            await player.Notify("Post Job", "Die Post wurde hier bereits abgeben!", NotificationType.INFO);
            return;
        }

        currentHouse.Returned = true;

        if (job.ReachedMaxHouses())
        {
            await player.Notify("Post Job",
                "Du hast alle Pakete abgegeben! Bring den Wagen zurück und erhalte dein Geld.", NotificationType.INFO);
            player.Emit("Client:PlayerModule:SetWaypoint", PostJobModel.StartPosition.X, PostJobModel.StartPosition.Y);
            MarkerStreamer.RemoveMarker(job.RouteMarker.Id);
            return;
        }

        var nextHouse = job.ReturnPoints.FirstOrDefault(x => !x.Returned);
        if (nextHouse == null) return;

        var house = await _houseController.GetHouse(nextHouse.HouseId);
        if (house == null) return;

        await player.Notify("Post Job", "Paket erfolgreich abgegeben!", NotificationType.SUCCESS);
        player.Emit("Client:PlayerModule:SetWaypoint", house.Position.X, house.Position.Y);

        MarkerStreamer.RemoveMarker(job.RouteMarker.Id);

        job.RouteMarker = new Marker(1, new Position(house.Position.X, house.Position.Y, house.Position.Z - 1), new Rgba(0, 0, 255, 255), 100, 0);
        MarkerStreamer.AddMarker(job.RouteMarker);
    }

    public async Task OnColshape(ClShape shape, IEntity entity, bool entered)
    {
        if (entity.Type != BaseObjectType.Player || shape == null!) return;

        ClPlayer player = (ClPlayer)entity;
        if (player == null!) return;

        if (shape.ShapeType != ColshapeType.POST_JOB_START) return;

        player.SetInteraction(Interactions.KEY_E, entered ? Interactions.E_POST_JOB_POST_JOIN : Interactions.NONE);
    }

    private bool PlayerIsInJob(int playerId)
    {
        return _postJobService.PostJobs.FirstOrDefault(x => x.RouteOwner == playerId) != null;
    }
}