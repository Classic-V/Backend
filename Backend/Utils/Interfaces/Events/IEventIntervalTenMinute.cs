namespace Backend.Utils.Interfaces.Events
{
	public interface IEventIntervalTenMinute : IEvent
	{
		Task OnEveryTenMinute();
	}
}