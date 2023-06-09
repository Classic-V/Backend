using AltV.Net.Data;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.PizzaDeliveryJob;
using Backend.Utils.Streamer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Database
{
    public class PizzaDeliveryJobModel
    {
        public static Position StartPosition { get; set; } = new Position(-1342.2065f, -871.8461f, 16.82788f);

        public int Id { get; set; }
        public PositionModel VehiclePosition { get;set; }
        public List<DeliveryReturnData> Deliveries { get; set; }
        public int MaxDeliveries { get; set; }
        public int MaxPrice { get; set; }

        [NotMapped] public int RouteOwner { get; set; }
        [NotMapped] public ClVehicle Vehicle { get; set; }
        [NotMapped] public Marker RouteMarker { get; set; }

        public PizzaDeliveryJobModel() { }
        public PizzaDeliveryJobModel(PositionModel vehiclePosition, List<DeliveryReturnData> deliveries, int maxDeliveries, int maxPrice)
        {
            VehiclePosition = vehiclePosition;
            Deliveries = deliveries;
            MaxDeliveries = maxDeliveries;
            MaxPrice = maxPrice;
        }

        public bool ReachedMaxHouses()
        {
            return (Deliveries.Where(x => x.IsDelivered).Count() >= MaxDeliveries);
        }

        public int ReturnedHouses()
        {
            return Deliveries.Where(x => x.IsDelivered).Count();

        }

        public void ResetRoute()
        {
            RouteOwner = 0;

            Deliveries.ForEach(delivery =>
            {
                delivery.IsDelivered = false;
            });

            Vehicle.Destroy();
            MarkerStreamer.RemoveMarker(RouteMarker.Id);
            RouteMarker = null!;
        }
    }
}
