using Backend.Utils.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Controllers.Door.Interface
{
    public interface IDoorController
    {
        void LoadDoor(DoorModel model);
        Task<DoorModel> GetDoor(int id);
    }
}
