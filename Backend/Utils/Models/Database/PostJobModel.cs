using System.ComponentModel.DataAnnotations.Schema;
using AltV.Net.Data;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.PostJob;
using Backend.Utils.Streamer;

namespace Backend.Utils.Models.Database;

public class PostJobModel
{
    public static readonly Position StartPosition = new(-424.03516f, -2789.5518f, 6.3809814f);

    public int Id { get; set; }
    public PositionModel VehicleStart { get; set; }
    public List<PostReturnData> ReturnPoints { get; set; }
    public int MaxPrice { get; set; }
    public int MaxHouses { get; set; }

    [NotMapped] public ClVehicle Vehicle { get; set; } = null!;
    [NotMapped] public int RouteOwner { get; set; } = 0;
    [NotMapped] public Marker RouteMarker { get; set; } = null!;

    public PostJobModel()
    {
    }

    public PostJobModel(PositionModel vehicleStart, List<PostReturnData> returnPoints, int maxHouses, int maxPrice)
    {
        VehicleStart = vehicleStart;
        ReturnPoints = returnPoints;
        MaxHouses = maxHouses;
        MaxPrice = maxPrice;
    }

    public bool ReachedMaxHouses()
    {
        return (ReturnPoints.Where(x => x.Returned).Count() >= MaxHouses);
    }

    public int ReturnedHouses()
    {
        return ReturnPoints.Where(x => x.Returned).Count();

    }

    public void ResetRoute()
    {
        ReturnPoints.ForEach(item =>
        {
            item.Returned = false;
        });

        RouteOwner = 0;
        Vehicle.Destroy();
        Vehicle = null!;

        MarkerStreamer.RemoveMarker(RouteMarker.Id);
        RouteMarker = null!;
    }
}