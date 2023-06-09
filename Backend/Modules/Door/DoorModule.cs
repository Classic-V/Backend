using AltV.Net;
using AltV.Net.Elements.Entities;
using Backend.Controllers.Event.Interface;
using Backend.Data;
using Backend.Services.Door.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Modules.Door
{
    public class DoorModule : Module<DoorModule>, IEventColshape
    {
        private readonly IDoorService _doorService;

        public DoorModule(IEventController eventController, IDoorService doorService) : base("Door")
        {
            _doorService = doorService;

            eventController.OnClient("Server:Door:Lock", Lock);
        }

        private async void Lock(ClPlayer player, string eventKey)
        {
            if (player.DbModel == null) return;

            var shape = player.CurrentShape;
            if (shape == null || shape.ShapeType != ColshapeType.DOOR) return;

            var id = shape.Id;
            var door = await _doorService.GetDoor(id);
            if (door == null) return;
            if (!door.Access.Contains(player.DbModel.Team)) return;

            door.Locked = !door.Locked;

            var msg = door.Locked ? "verschlossen" : "geöffnet";

            await player.Notify("Tür", $"Du hast die Tür {msg}.", door.Locked ? NotificationType.ERROR : NotificationType.SUCCESS);
            await _doorService.UpdateDoor(door);

            Alt.EmitAllClients("Client:DoorModule:Lock", JsonConvert.SerializeObject(door));
        }

        public async Task OnColshape(ClShape shape, IEntity entity, bool entered)
        {
            if (entity.Type != BaseObjectType.Player || !entered || shape == null) return;
            if (shape.ShapeType != ColshapeType.DOOR) return;

            ClPlayer player = (ClPlayer) entity;
            if (player == null) return;

            player.SetInteraction(Interactions.KEY_L, Interactions.L_DOOR_LOCK);
        }
    }
}
