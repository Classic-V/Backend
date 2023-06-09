using Backend.Utils.Models.Entities;

namespace Backend.Utils.Interfaces.Events
{
	public interface IEventPlayerDisconnect : IEvent
	{
		Task OnPlayerDisconnect(ClPlayer player, string reason);
	}
}