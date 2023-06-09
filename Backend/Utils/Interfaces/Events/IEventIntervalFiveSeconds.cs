namespace Backend.Utils.Interfaces.Events
{
	public interface IEventIntervalFiveSeconds : IEvent
	{
		Task OnEveryFiveSeconds();
	}
}