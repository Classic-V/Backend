namespace Backend.Utils.Interfaces.Events
{
	public interface IEventIntervalMinute : IEvent
	{
		Task OnEveryMinute();
	}
}