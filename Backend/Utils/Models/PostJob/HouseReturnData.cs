using AltV.Net.Data;
using Newtonsoft.Json;

namespace Backend.Utils.Models.PostJob;

public class PostReturnData
{
    public int HouseId { get; set; }
    [JsonIgnore] public bool Returned { get; set; } = false;

    public PostReturnData() {}

    public PostReturnData(int houseId)
    {
        HouseId = houseId;
    }
}