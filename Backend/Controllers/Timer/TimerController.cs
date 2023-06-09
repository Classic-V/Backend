using System.Timers;
using Backend.Controllers.Timer.Interface;

namespace Backend.Controllers.Timer;

public class TimerController : ITimerController
{
    private readonly List<System.Timers.Timer> _timers = new ();
    
    public Task Add(float duration, ElapsedEventHandler handler)
    {
        var timer = new System.Timers.Timer(duration);
        timer.Elapsed += handler;
        timer.Start();

        _timers.Add(timer);
        
        return Task.CompletedTask;
    }
}