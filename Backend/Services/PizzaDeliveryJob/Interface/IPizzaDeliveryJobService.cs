using Backend.Utils.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services.PizzaDeliveryJob.Interface
{
    public interface IPizzaDeliveryJobService
    {
        List<PizzaDeliveryJobModel> PizzaDeliveryJobs { get; }

        Task AddPizzaDeliveryJob(PizzaDeliveryJobModel model);
        Task DeletePizzaDeliveryJob(PizzaDeliveryJobModel model);
        Task UpdatePizzaDeliveryJob(PizzaDeliveryJobModel model);
    }
}
