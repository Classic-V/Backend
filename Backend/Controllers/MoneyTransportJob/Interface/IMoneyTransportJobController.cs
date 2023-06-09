using Backend.Utils.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Controllers.MoneyTransportJob.Interface
{
    public interface IMoneyTransportJobController
    {
        void LoadMoneyTransportJob(MoneyTransportJobModel model);
        Task<MoneyTransportJobModel> GetMoneyTransportJob(int id);
        bool PlayerIsInMoneyJob(int playerId);
    }
}
