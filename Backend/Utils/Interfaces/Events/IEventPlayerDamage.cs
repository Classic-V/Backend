using AltV.Net.Elements.Entities;
using Backend.Utils.Models.Entities;

namespace Backend.Utils.Interfaces.Events
{
	public interface IEventPlayerDamage : IEvent
	{
		Task OnPlayerDamage(ClPlayer player, IEntity attacker, ushort oldHealth, ushort oldArmor,
			ushort oldMaxHealth, ushort oldMaxArmor, uint weapon, ushort healthDamage, ushort armorDamage);
	}
}