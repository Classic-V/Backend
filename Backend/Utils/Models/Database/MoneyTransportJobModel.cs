using AltV.Net.Data;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.MoneyTransportJob;
using Backend.Utils.Streamer;
using Discord.Rest;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Utils.Models.Database
{
    public class MoneyTransportJobModel
    {
        public static Position StartPosition = new Position(-6.896702f, -654.6462f, 33.441772f);

        public int Id { get; set; }
        public PositionModel VehiclePosition { get; set; }
        public List<BankReturnData> Ids { get; set; }
        public int MaxPrice { get; set; }

        [NotMapped] public int RouteOwner { get; set; } = 0;
        [NotMapped] public ClVehicle Vehicle { get; set; } = null!;
        [NotMapped] public Marker RouteMarker { get; set; } = null!;

        public MoneyTransportJobModel() { }
        public MoneyTransportJobModel(PositionModel vehicleStart, List<BankReturnData> ids, int maxPrice)
        {
            VehiclePosition = vehicleStart;
            Ids = ids;
            MaxPrice = maxPrice;
        }

        public bool ReturnedAllNotes()
        {
            return (Ids.Where(x => x.Returned).Count() >= Ids.Count);
        }

        public int ReturnedNotes()
        {
            return Ids.Where(x => x.Returned).Count();
        }

        public void ResetRoute()
        {
            Vehicle.Destroy();
            Vehicle = null!;
            RouteOwner = 0;

            Ids.ForEach(id =>
            {
                id.Returned = false;
            });

            MarkerStreamer.RemoveMarker(RouteMarker.Id);
            RouteMarker = null!;
        }
    }
}
