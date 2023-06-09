using Backend.Utils.Models.Entities;

namespace Backend.Utils.Interfaces.Events
{
	public interface IEventPlayerWeaponSwitch : IEvent
	{
		Task OnPlayerWeaponSwitch(ClPlayer player, uint oldWeapon, uint newWeapon);
	}
}