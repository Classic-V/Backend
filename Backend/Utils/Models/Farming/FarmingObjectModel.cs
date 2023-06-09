using AltV.Net.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Utils.Models.Farming
{
	public class FarmingObjectModel
	{
		public Position Position { get; set; }
		[NotMapped]
		public int Health { get; set; } = 100;
	}
}