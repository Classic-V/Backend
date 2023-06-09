using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Controllers.Event.Interface
{
	public interface IEventController
	{
		void OnClient(string eventName, Action<ClPlayer, string> action);

		void OnClient<T1>(string eventName, Action<ClPlayer, string, T1> action);
		void OnClient<T1, T2>(string eventName, Action<ClPlayer, string, T1, T2> action);
		void OnClient<T1, T2, T3>(string eventName, Action<ClPlayer, string, T1, T2, T3> action);
		void OnClient<T1, T2, T3, T4>(string eventName, Action<ClPlayer, string, T1, T2, T3, T4> action);
		void OnClient<T1, T2, T3, T4, T5>(string eventName, Action<ClPlayer, string, T1, T2, T3, T4, T5> action);
		void OnClient<T1, T2, T3, T4, T5, T6>(string eventName, Action<ClPlayer, string, T1, T2, T3, T4, T5, T6> action);
		void OnClient<T1, T2, T3, T4, T5, T6, T7>(string eventName, Action<ClPlayer, string, T1, T2, T3, T4, T5, T6, T7> action);
		void OnClient<T1, T2, T3, T4, T5, T6, T7, T8>(string eventName, Action<ClPlayer, string, T1, T2, T3, T4, T5, T6, T7, T8> action);
		void OnClient<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string eventName, Action<ClPlayer, string, T1, T2, T3, T4, T5, T6, T7, T8, T9> action);
		void OnClient<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string eventName, Action<ClPlayer, string, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action);
		void OnClient<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string eventName, Action<ClPlayer, string, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action);
		void OnClient<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string eventName, Action<ClPlayer, string, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action);
	}
}