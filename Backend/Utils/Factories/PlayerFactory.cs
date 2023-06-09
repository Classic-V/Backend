using AltV.Net;
using AltV.Net.Elements.Entities;
using Backend.Utils.Models.Entities;

namespace Backend.Utils.Factories
{
	public class PlayerFactory : IEntityFactory<IPlayer>
	{
		public IPlayer Create(ICore core, IntPtr entityPointer, ushort id)
		{
			return new ClPlayer(core, entityPointer, id);
		}
	}
}