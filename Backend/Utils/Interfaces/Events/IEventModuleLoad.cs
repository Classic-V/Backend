namespace Backend.Utils.Interfaces.Events
{
	public interface IEventModuleLoad : IEvent
	{
		Task OnLoad();
	}
}