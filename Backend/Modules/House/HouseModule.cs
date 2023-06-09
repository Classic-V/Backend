using Backend.Controllers.Event.Interface;
using Backend.Controllers.House.Interface;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Player.Client;
using System.Diagnostics.Metrics;
using System.Runtime.InteropServices;
using AltV.Net.Elements.Entities;
using Backend.Utils.Enums;
using Backend.Utils.Interfaces.Events;
using Backend.Data;
using Backend.Services.Account.Interface;
using Backend.Utils.Models.Database;
using Backend.Services.PostJob.Interface;
using Newtonsoft.Json;
using Autofac.Features.OwnedInstances;
using Backend.Services.PizzaDeliveryJob.Interface;
using Autofac.Core;

namespace Backend.Modules.House;

public class HouseModule : Module<HouseModule>, IEventColshape
{
    private readonly IHouseController _houseController;
    private readonly IAccountService _accountService;

    private readonly IPostJobService _postJobService;
    private readonly IPizzaDeliveryJobService _pizzaDeliveryJobService;

    public HouseModule(IEventController eventController, IHouseController houseController, IAccountService accountService, IPostJobService postJobService, IPizzaDeliveryJobService pizzaDeliveryJobService) : base("House")
    {
        _houseController = houseController;
        _accountService = accountService;
        _postJobService = postJobService;
        _pizzaDeliveryJobService = pizzaDeliveryJobService;

        eventController.OnClient("Server:House:Open", Open);
        eventController.OnClient<int>("Server:House:Enter", Enter);
        eventController.OnClient("Server:House:Lock", Lock);
        eventController.OnClient<int, string>("Server:House:Rent", Rent);
    }

    private async void Open(ClPlayer player, string eventKey)
    {
        if (player == null! || player.DbModel == null! || player.CurrentShape == null) return;

        var shape = player.CurrentShape;
        if (shape == null || shape.ShapeType != ColshapeType.HOUSE) return;

        var id = player.CurrentShape.Id;
        var house = await _houseController.GetHouse(id);
        if (house == null || house.Locked) return;

        shape.GetData("Entry", out bool result);
        if (!result)
        {
            player.IsInInterior = false;
            player.OutsideInteriorPosition = house.Position;
            await player.SetPosition(house.Position);
            await player.SetDimension(0);
            return;
        }

        var list = new List<ClientNativeMenuItem>
        {
            new("Haus betreten", true, "Server:House:Enter", id)
        };

        var menu = new ClientNativeMenu($"Haus ({id})", list);
        await player.ShowNativeMenu(true, menu);
    }

    private async void Enter(ClPlayer player, string eventKey, int id)
    {
        var house = await _houseController.GetHouse(id);
        if (house == null) return;

        var housePosition = await _houseController.GetHouseInsidePosition(id);  
        if (housePosition == null) return;

        player.IsInInterior = true;
        player.OutsideInteriorPosition = house.Position;
        await player.SetPosition(housePosition.Position);
        await player.SetDimension(house.Id + 50);
    }

    private async void Lock(ClPlayer player, string eventKey)
    {
        if (player == null! || player.DbModel == null! || player.CurrentShape == null!) return;

        var houseId = player.CurrentShape.Id;
        if (houseId <= 0) return;

        var house = await _houseController.GetHouse(houseId);
        if (house == null) return;

        if (!house.CanAccess(player.DbModel.Id)) return;
        house.Locked = !house.Locked;

        string msg = house.Locked ? "verschlossen" : "aufgeschlossen";
        await player.Notify("Haus", $"Du hast das Haus {msg}", house.Locked ? NotificationType.ERROR : NotificationType.SUCCESS);
    }

    private async void Rent(ClPlayer player, string eventKey, int houseId, string targetName)
    {
        if (player.DbModel == null! || player == null! || houseId <= 0) return;

        if (targetName.Length <= 0)
        {
            await player.Notify("Haus", "Du musst einen Spieler Namen angeben.", NotificationType.ERROR);
            return;
        }

        ClPlayer target = ClPlayer.All.FirstOrDefault(x => x.DbModel.Name.Equals(targetName))!;
        if (target == null)
        {
            await player.Notify("Haus", "Spieler nicht gefunden!", NotificationType.ERROR);
            return;
        }

        _houseController.AddRenter(player, houseId, targetName, 1);
    }

    public async Task OnColshape(ClShape shape, IEntity entity, bool entered)
    {
        if (entity.Type != BaseObjectType.Player) return;

        ClPlayer player = (ClPlayer) entity;
        if (player == null!) return;
        if (!entered) return;
        if (shape.ShapeType != ColshapeType.HOUSE) return;

        if (PlayerIsInJob(player.DbModel.Id))
        {
            player.SetInteraction(Interactions.KEY_E, Interactions.E_POST_JOB_POST_RETURN);
            return;
        }

        if (PlayerIsInPizzaJob(player.DbModel.Id))
        {
            player.SetInteraction(Interactions.KEY_E, Interactions.E_PIZZA_JOB_RETURN);
            return;
        }

        player.SetInteraction(Interactions.KEY_E, entered ? Interactions.E_HOUSE : Interactions.NONE);
        player.SetInteraction(Interactions.KEY_L, entered ? Interactions.L_HOUSE : Interactions.NONE);

        shape.GetData("Entry", out bool result);
        if (!result) return;

        int houseId = shape.Id;
        if (houseId == 0) return;

        var house = await _houseController.GetHouse(houseId);
        if (house == null) return;
        
        if (house.Owner != 0)
        {
            AccountModel owner = _accountService.GetAccount(house.Owner)!;
            if (owner == null!) return;

            await player.Notify($"Haus ({house.Id}) ", $"Haus Besitzer: {owner.Name}", house.Locked ? NotificationType.ERROR : NotificationType.SUCCESS);
            return;
        }

        await player.Notify($"Haus ({house.Id})", $"Das Haus hat kein Besitzer. ({house.Price}$)", NotificationType.INFO);
    }

    private bool PlayerIsInJob(int playerId)
    {
        return _postJobService.PostJobs.FirstOrDefault(x => x.RouteOwner == playerId) != null;
    }

    private bool PlayerIsInPizzaJob(int playerId)
    {
        return _pizzaDeliveryJobService.PizzaDeliveryJobs.FirstOrDefault(x => x.RouteOwner == playerId) != null;
    }
}