using Backend.Utils.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Garage.Client
{
	public class ClientVehicleModel
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Note { get; set; } = string.Empty;
		public string Plate { get; set; } = string.Empty;
		public bool Parked { get; set; }

		public ClientVehicleModel(VehicleModel model, string name)
		{
			Id = model.Id;
			Name = name;
			Note = model.Note;
			Plate = model.Plate;
			Parked = model.Parked;
		}
	}
}