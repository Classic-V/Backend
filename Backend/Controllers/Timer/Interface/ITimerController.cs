using System.Timers;

namespace Backend.Controllers.Timer.Interface;

public interface ITimerController
{
    Task Add(float duration, ElapsedEventHandler handler);
}