using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Controllers.JeweleryHeist.Interface
{
	public interface IJeweleryHeistController
	{
		bool IsBeingRobbed { get; }
		bool HasBeenAttacked { get; }
		Task<bool> StartHeist(ClPlayer player);
		Task UnlockLoot(ClPlayer player);
		Task StopHeist();
	}
}