using AltV.Net;
using AltV.Net.Elements.Entities;
using Backend.Utils.Models.Entities;

namespace Backend.Utils.Factories
{
	public class VehicleFactory : IEntityFactory<IVehicle>
	{
		public IVehicle Create(ICore core, IntPtr entityPointer, ushort id)
		{
			return new ClVehicle(core, entityPointer, id);
		}
	}
}