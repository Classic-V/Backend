using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Ammo
{
	public class ShotgunAmmo : AmmoItemBase
	{
		public override int Id => 2003;

		public ShotgunAmmo() : base("Schortflinten Munition", 0.2f, 25, Enums.WeaponType.SHOTGUN)
		{
		}
	}
}