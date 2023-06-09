using AltV.Net.Elements.Entities;
using Backend.Utils.Models.Entities;

namespace Backend.Utils.Interfaces.Events
{
	public interface IEventPlayerDeath : IEvent
	{
		Task OnPlayerDeath(ClPlayer player, IEntity? killer, uint weapon);
	}
}