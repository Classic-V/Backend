using Backend.Utils.Models.Database;
using Backend.Utils.Models.VehicleShop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Controllers.VehicleShop.Interface
{
	public interface IVehicleShopController
	{
		void LoadVehicleShop(VehicleShopModel model);
		void LoadVehicleShopItem(VehicleShopItem model);
	}
}