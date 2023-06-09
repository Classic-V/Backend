using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Utils.Models.Database;

namespace Backend.Services.Garage.Interface
{
    public interface IGarageService
    {
        List<GarageModel> Garages { get; }
        Task AddGarage(GarageModel garage);
        Task<GarageModel?> GetGarage(int id);
        Task UpdateGarage(GarageModel garage);
    }
}
