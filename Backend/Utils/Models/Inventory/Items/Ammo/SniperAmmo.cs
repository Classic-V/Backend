using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Ammo
{
	public class SniperAmmo : AmmoItemBase
	{
		public override int Id => 2004;

		public SniperAmmo() : base("Sniper Munition", 0.2f, 25, Enums.WeaponType.SNIPER)
		{
		}
	}
}