using Backend.Utils.Models.Entities;

namespace Backend.Utils.Interfaces.Events
{
	public interface IEventVehicleEnter : IEvent
	{
		Task OnVehicleEnter(ClVehicle vehicle, ClPlayer player, byte seat);
	}
}