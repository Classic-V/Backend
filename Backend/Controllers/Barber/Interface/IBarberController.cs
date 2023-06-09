using Backend.Utils.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Controllers.Barber.Interface
{
    public interface IBarberController
    {
        void LoadBarbers(BarberModel model);
    }
}
