using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Utils.Models.Database;

namespace Backend.Services.Gangwar.Interface
{
    public interface IGangwarService
    {
        List<GangwarModel> Gangwar { get; }

        GangwarModel GetGangwar(int id);
        Task AddGangwar(GangwarModel model);
        Task UpdateGangwar(GangwarModel model);
        Task UpdateGangwars();

	}
}
