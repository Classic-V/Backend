using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Ammo
{
	public class PistolAmmo : AmmoItemBase
	{
		public override int Id => 2001;

		public PistolAmmo() : base("Pistolen Munition", 0.2f, 25, Enums.WeaponType.PISTOL)
		{
		}
	}
}