using Backend.Utils.Models.Entities;

namespace Backend.Utils.Interfaces.Events
{
	public interface IEventPlayerConnect : IEvent
	{
		Task OnPlayerConnect(ClPlayer player, string reason);
	}
}