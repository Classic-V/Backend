using AltV.Net;
using Backend.Controllers.PizzaDeliveryJob.Interface;
using Backend.Services.PizzaDeliveryJob.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Streamer;

namespace Backend.Controllers.PizzaDeliveryJob
{
    public class PizzaDeliveryJobController : IPizzaDeliveryJobController
    {
        private readonly IPizzaDeliveryJobService _pizzaDeliveryJobService;

        public PizzaDeliveryJobController(IPizzaDeliveryJobService pizzaDeliveryJobService)
        {
            _pizzaDeliveryJobService = pizzaDeliveryJobService;

            _pizzaDeliveryJobService.PizzaDeliveryJobs.ForEach(LoadPizzaDeliveryJob);
            PedStreamer.AddObject(new Ped(0x6F4747CE, PizzaDeliveryJobModel.StartPosition.X, PizzaDeliveryJobModel.StartPosition.Y, PizzaDeliveryJobModel.StartPosition.Z - 1, 180, true, true, 0));

            var blip = Alt.CreateBlip(AltV.Net.Elements.Entities.BlipType.Destination, PizzaDeliveryJobModel.StartPosition);
            blip.ShortRange = true;
            blip.Sprite = 478;
            blip.Name = "Pizza Lieferant";
        }

        public void LoadPizzaDeliveryJob(PizzaDeliveryJobModel model)
        {
            var shape = (ClShape)Alt.CreateColShapeSphere(PizzaDeliveryJobModel.StartPosition, 2f);
            shape.Id = model.Id;
            shape.Dimension = 0;
            shape.Size = 2f;
            shape.ShapeType = ColshapeType.PIZZA_DELIVERY_JOB;
        }

        public Task<PizzaDeliveryJobModel> GetPizzaDeliveryJob(int id)
        {
            return Task.FromResult(_pizzaDeliveryJobService.PizzaDeliveryJobs.FirstOrDefault(x => x.Id == id)!);
        }
    }
}
