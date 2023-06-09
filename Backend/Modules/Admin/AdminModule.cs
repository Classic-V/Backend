using Backend.Controllers.Event.Interface;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Player;

namespace Backend.Modules.Admin
{
	public class AdminModule : Module<AdminModule>
	{
		public AdminModule(IEventController eventController) : base("Admin")
		{
			//eventController.OnClient("Server:Admin:ToggleDuty", ToggleAduty);
		}
	}
}