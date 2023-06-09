using Backend.Services.PizzaDeliveryJob.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services.PizzaDeliveryJob
{
    public class PizzaDeliveryJobService : IPizzaDeliveryJobService
    {
        public List<PizzaDeliveryJobModel> PizzaDeliveryJobs { get; private set; }

        public PizzaDeliveryJobService()
        {
            var ctx = new DBContext();
            PizzaDeliveryJobs = new List<PizzaDeliveryJobModel>(ctx.PizzaDeliveryJobs);
        }

        public async Task AddPizzaDeliveryJob(PizzaDeliveryJobModel model)
        {
            PizzaDeliveryJobs.Add(model);

            var ctx = new DBContext();
            ctx.PizzaDeliveryJobs.Add(model);
            await ctx.SaveChangesAsync();
        }

        public async Task DeletePizzaDeliveryJob(PizzaDeliveryJobModel model)
        {
            PizzaDeliveryJobs.Remove(model);

            var ctx = new DBContext();
            ctx.PizzaDeliveryJobs.Remove(model);
            await ctx.SaveChangesAsync();
        }

        public async Task UpdatePizzaDeliveryJob(PizzaDeliveryJobModel model)
        {
            var ctx = new DBContext();
            ctx.PizzaDeliveryJobs.Update(model);
            await ctx.SaveChangesAsync();
        }
    }
}
