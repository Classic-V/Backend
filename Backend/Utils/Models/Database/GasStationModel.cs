using AltV.Net.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Utils.Models.Database
{
	public class GasStationModel
	{
		public int Id { get; set; }
		public Position Position { get; set; }

		[NotMapped]
		public int Price { get; set; } = new Random().Next(20, 50);
	}
}