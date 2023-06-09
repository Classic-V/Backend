using Backend.Utils.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services.Barber.Interface
{
    public interface IBarberService
    {
        List<BarberModel> Barbers { get; }
        List<BarberItemModel> BarberItems { get; }

        Task<BarberModel> GetBarber(int id);
        Task<BarberItemModel> GetBarberItem(int id);
        Task AddBarber(BarberModel model);
        Task UpdateBarber(BarberModel model);
        Task DeleteBarber(BarberModel model);
        Task AddBarberItem(BarberItemModel model);
        Task UpdateBarberItem(BarberItemModel model);
        Task DeleteBarberItem(BarberItemModel model);

    }
}
