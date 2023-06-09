using Backend.Utils.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services.MoneyTransportJob.Interface
{
    public interface IMoneyTransportJobService
    {
        List<MoneyTransportJobModel> MoneyTransportJobs { get; }
        Task<MoneyTransportJobModel> GetMoneyTransportJob(int id);
        Task AddMoneyTransportJob(MoneyTransportJobModel model);
        Task DeleteMoneyTransportJob(MoneyTransportJobModel model);
        Task UpdateMoneyTransportJob(MoneyTransportJobModel model);
    }
}
