using Backend.Utils.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Garage.Client
{
	public class ClientGarageModel
	{
		public int Id { get; set; } = -1;
		public List<ClientVehicleModel> Vehicles { get; set; } = new();

		public ClientGarageModel() { }

		public ClientGarageModel(int id)
		{
			Id = id;
		}

		public ClientGarageModel(int id, List<(VehicleModel Vehicle, VehicleInfoModel Info)> vehicles)
		{
			Id = id;
			vehicles.ForEach(x => Vehicles.Add(new ClientVehicleModel(x.Vehicle, x.Info.Name)));
		}
	}
}