using Backend.Services.MoneyTransportJob.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services.MoneyTransportJob
{
    public class MoneyTransportJobService : IMoneyTransportJobService
    {
        public List<MoneyTransportJobModel> MoneyTransportJobs { get; set; }

        public MoneyTransportJobService()
        {
            var ctx = new DBContext();
            MoneyTransportJobs = new List<MoneyTransportJobModel>(ctx.MoneyTransportJob);
        }

        public Task<MoneyTransportJobModel> GetMoneyTransportJob(int id)
        {
            return Task.FromResult(MoneyTransportJobs.FirstOrDefault(x => x.Id == id))!;
        }

        public async Task AddMoneyTransportJob(MoneyTransportJobModel model)
        {
            MoneyTransportJobs.Add(model);

            var ctx = new DBContext();
            ctx.MoneyTransportJob.Add(model);
            await ctx.SaveChangesAsync();
        }

        public async Task DeleteMoneyTransportJob(MoneyTransportJobModel model)
        {
            MoneyTransportJobs.Remove(model);

            var ctx = new DBContext();
            ctx.MoneyTransportJob.Remove(model);
            await ctx.SaveChangesAsync();
        }

        public async Task UpdateMoneyTransportJob(MoneyTransportJobModel model)
        {
            var ctx = new DBContext();
            ctx.MoneyTransportJob.Update(model);
            await ctx.SaveChangesAsync();
        }
    }
}
