using Backend.Utils.Models.Entities;

namespace Backend.Utils.Interfaces.Events
{
	public interface IEventVehicleExit : IEvent
	{
		Task OnVehicleExit(ClVehicle vehicle, ClPlayer player, byte seat);
	}
}