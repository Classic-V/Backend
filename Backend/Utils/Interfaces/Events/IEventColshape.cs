using AltV.Net.Elements.Entities;
using Backend.Utils.Models.Entities;

namespace Backend.Utils.Interfaces.Events
{
	public interface IEventColshape : IEvent
	{
		Task OnColshape(ClShape shape, IEntity entity, bool entered);
	}
}