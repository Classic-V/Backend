using Backend.Utils.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Controllers.PizzaDeliveryJob.Interface
{
    public interface IPizzaDeliveryJobController
    {
        void LoadPizzaDeliveryJob(PizzaDeliveryJobModel model);
        Task<PizzaDeliveryJobModel> GetPizzaDeliveryJob(int id);
    }
}
