using Backend.Services.Door.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services.Door
{
    public class DoorService : IDoorService
    {
        public List<DoorModel> Doors { get; private set; }

        public DoorService()
        {
            var ctx = new DBContext();
            Doors = new List<DoorModel>(ctx.Door);
        }

        public async Task AddDoor(DoorModel model)
        {
            Doors.Add(model);

            var ctx = new DBContext();
            ctx.Door.Add(model);
            await ctx.SaveChangesAsync();
        }

        public async Task DeleteDoor(DoorModel model)
        {
            Doors.Remove(model);

            var ctx = new DBContext();
            ctx.Door.Remove(model);
            await ctx.SaveChangesAsync();
        }

        public async Task UpdateDoor(DoorModel model)
        {
            var ctx = new DBContext();
            ctx.Door.Update(model);
            await ctx.SaveChangesAsync();
        }

        public Task<DoorModel> GetDoor(int id)
        {
            return Task.FromResult(Doors.FirstOrDefault(x => x.Id == id))!;
        }
    }
}
