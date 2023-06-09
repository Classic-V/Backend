using Backend.Utils.Models.Database;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services.Door.Interface
{
    public interface IDoorService
    {
        List<DoorModel> Doors { get; }
        Task<DoorModel> GetDoor(int id);
        Task AddDoor(DoorModel model);
        Task UpdateDoor(DoorModel model);
        Task DeleteDoor(DoorModel model);
    }
}
