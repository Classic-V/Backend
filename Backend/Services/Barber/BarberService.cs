using Backend.Services.Barber.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services.Barber
{
    public class BarberService : IBarberService
    {
        public List<BarberModel> Barbers { get; private set; }

        public List<BarberItemModel> BarberItems { get; private set; }

        public BarberService()
        {
            var context = new DBContext();
            Barbers = new List<BarberModel>(context.Barbers);
            BarberItems = new List<BarberItemModel>(context.BarberItems);
        }


        public async Task AddBarber(BarberModel model)
        {
            Barbers.Add(model);

            var context = new DBContext();
            context.Barbers.Add(model);
            await context.SaveChangesAsync();
        }

        public async Task AddBarberItem(BarberItemModel model)
        {
            BarberItems.Add(model);

            var context = new DBContext();
            context.BarberItems.Add(model);
            await context.SaveChangesAsync();
        }

        public async Task DeleteBarber(BarberModel model)
        {
            Barbers.Remove(model);

            var context = new DBContext();
            context.Barbers.Remove(model);
            await context.SaveChangesAsync();
        }

        public async Task DeleteBarberItem(BarberItemModel model)
        {
            BarberItems.Remove(model);

            var context = new DBContext();
            context.BarberItems.Remove(model);
            await context.SaveChangesAsync();
        }

        public Task<BarberModel> GetBarber(int id)
        {
            return Task.FromResult(Barbers.FirstOrDefault(x => x.Id == id))!;
        }

        public Task<BarberItemModel> GetBarberItem(int id)
        {
            return Task.FromResult(BarberItems.FirstOrDefault(x => x.Id == id))!;
        }

        public async Task UpdateBarber(BarberModel model)
        {
            var context = new DBContext();
            context.Barbers.Update(model);
            await context.SaveChangesAsync();
        }

        public async Task UpdateBarberItem(BarberItemModel model)
        {
            var context = new DBContext();
            context.BarberItems.Update(model);
            await context.SaveChangesAsync();
        }
    }
}
