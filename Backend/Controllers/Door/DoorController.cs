using AltV.Net;
using AltV.Net.Data;
using Backend.Controllers.Door.Interface;
using Backend.Services.Door.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Controllers.Door
{
    public class DoorController : IDoorController
    {
        private readonly IDoorService _doorService;

        public DoorController(IDoorService doorService)
        {
            _doorService = doorService;

            _doorService.Doors.ForEach(LoadDoor);
        }

        public void LoadDoor(DoorModel model)
        {
            var shape = (ClShape)Alt.CreateColShapeSphere(model.Position, model.Radius);
            shape.Id = model.Id;
            shape.ShapeType = ColshapeType.DOOR;
            shape.Dimension = 0;
            shape.Size = model.Radius;
        }

        public async Task<DoorModel> GetDoor(int id)
        {
            return await _doorService.GetDoor(id);
        }
    }
}
