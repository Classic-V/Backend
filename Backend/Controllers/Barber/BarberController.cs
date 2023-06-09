using AltV.Net;
using AltV.Net.CApi.ClientEvents;
using AltV.Net.Elements.Entities;
using Backend.Controllers.Barber.Interface;
using Backend.Services.Barber.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Controllers.Barber
{
    public class BarberController : IBarberController
    {
        private readonly IBarberService _barberService;

        public BarberController(IBarberService barberService)
        {
            _barberService = barberService;

            _barberService.Barbers.ForEach(LoadBarbers);
        }

        public void LoadBarbers(BarberModel model)
        {
            var blip = Alt.CreateBlip(BlipType.Destination, model.Position);
            blip.Dimension = 0;
            blip.ShortRange = true;
            blip.Sprite = 71;
            blip.Name = "Friseur";

            var shape = (ClShape) Alt.CreateColShapeSphere(model.Position, model.Radius);
            shape.Dimension = 0;
            shape.Id = model.Id;
            shape.Size = model.Radius;
            shape.ShapeType = ColshapeType.BARBER;
        }
    }
}
