using System.Reflection;
using Backend.Controllers.Event.Interface;
using Backend.Utils.Models;

namespace Backend.Modules.Ban;

public class BanModule : Module<BanModule>
{
    private IEventController _eventController;
    public BanModule(IEventController eventController) : base("Ban")
    {
    }
}