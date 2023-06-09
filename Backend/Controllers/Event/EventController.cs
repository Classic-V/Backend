using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using Backend.Controllers.Event.Interface;
using Backend.Utils.Models.Entities;

namespace Backend.Controllers.Event
{
	public class EventController : IEventController
	{
		public EventController()
		{
			AltAsync.OnPlayerEvent += EventReceived;
		}

		public Task EventReceived(IPlayer iplayer, string eventName, object[] args)
		{
			var player = (ClPlayer)iplayer;

			if ((string)args[0] != player.EventKey)
			{
				// Todo: Ban player
				player.Kick("Du wurdest gekicked! Grund: Executor");
			}

			return Task.CompletedTask;
		}

		public void OnClient(string eventName, Action<ClPlayer, string> action)
		{
			AltAsync.OnClient(eventName, action);
		}

		public void OnClient<T1>(string eventName, Action<ClPlayer, string, T1> action)
		{
			AltAsync.OnClient(eventName, action);
		}

		public void OnClient<T1, T2>(string eventName, Action<ClPlayer, string, T1, T2> action)
		{
			AltAsync.OnClient(eventName, action);
		}

		public void OnClient<T1, T2, T3>(string eventName, Action<ClPlayer, string, T1, T2, T3> action)
		{
			AltAsync.OnClient(eventName, action);
		}

		public void OnClient<T1, T2, T3, T4>(string eventName, Action<ClPlayer, string, T1, T2, T3, T4> action)
		{
			AltAsync.OnClient(eventName, action);
		}

		public void OnClient<T1, T2, T3, T4, T5>(string eventName, Action<ClPlayer, string, T1, T2, T3, T4, T5> action)
		{
			AltAsync.OnClient(eventName, action);
		}

		public void OnClient<T1, T2, T3, T4, T5, T6>(string eventName, Action<ClPlayer, string, T1, T2, T3, T4, T5, T6> action)
		{
			AltAsync.OnClient(eventName, action);
		}

		public void OnClient<T1, T2, T3, T4, T5, T6, T7>(string eventName, Action<ClPlayer, string, T1, T2, T3, T4, T5, T6, T7> action)
		{
			AltAsync.OnClient(eventName, action);
		}

		public void OnClient<T1, T2, T3, T4, T5, T6, T7, T8>(string eventName, Action<ClPlayer, string, T1, T2, T3, T4, T5, T6, T7, T8> action)
		{
			AltAsync.OnClient(eventName, action);
		}

		public void OnClient<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string eventName, Action<ClPlayer, string, T1, T2, T3, T4, T5, T6, T7, T8, T9> action)
		{
			AltAsync.OnClient(eventName, action);
		}

		public void OnClient<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string eventName, Action<ClPlayer, string, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action)
		{
			AltAsync.OnClient(eventName, action);
		}

		public void OnClient<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string eventName, Action<ClPlayer, string, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action)
		{
			AltAsync.OnClient(eventName, action);
		}

		public void OnClient<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string eventName, Action<ClPlayer, string, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action)
		{
			AltAsync.OnClient(eventName, action);
		}
	}
}