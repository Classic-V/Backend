using AltV.Net;
using Backend.Controllers.MoneyTransportJob.Interface;
using Backend.Services.MoneyTransportJob.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Streamer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Controllers.MoneyTransportJob
{
    public class MoneyTransportJobController : IMoneyTransportJobController
    {
        private readonly IMoneyTransportJobService _moneyTransportJobService;

        public MoneyTransportJobController(IMoneyTransportJobService moneyTransportJobService)
        {
            _moneyTransportJobService = moneyTransportJobService;

            _moneyTransportJobService.MoneyTransportJobs.ForEach(LoadMoneyTransportJob);
            PedStreamer.AddObject(new Ped(0xEA969C40, MoneyTransportJobModel.StartPosition.X, MoneyTransportJobModel.StartPosition.Y, MoneyTransportJobModel.StartPosition.Z - 1, 180, true, true, 0));

            var blip = Alt.CreateBlip(AltV.Net.Elements.Entities.BlipType.Destination, MoneyTransportJobModel.StartPosition);
            blip.ShortRange = true;
            blip.Sprite = 477;
            blip.Name = "Geldtransporter";
        }

        public void LoadMoneyTransportJob(MoneyTransportJobModel model)
        {
            var startShape = (ClShape)Alt.CreateColShapeSphere(MoneyTransportJobModel.StartPosition, 2f);
            startShape.Id = model.Id;
            startShape.Size = 2f;
            startShape.Dimension = 0;
            startShape.ShapeType = ColshapeType.MONEY_TRANSPORT_JOB;

            MarkerStreamer.AddMarker(new Marker(1, MoneyTransportJobModel.StartPosition, new AltV.Net.Data.Rgba(0, 0, 255, 255), 20, 0));
        }

        public async Task<MoneyTransportJobModel> GetMoneyTransportJob(int id)
        {
            return await _moneyTransportJobService.GetMoneyTransportJob(id);
        }

        public bool PlayerIsInMoneyJob(int playerId)
        {
            return _moneyTransportJobService.MoneyTransportJobs.FirstOrDefault(x => x.RouteOwner == playerId) != null;
        }
    }
}
