using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Ammo
{
	public class RifleAmmo : AmmoItemBase
	{
		public override int Id => 2000;

		public RifleAmmo() : base("Sturmgewehr Munition", 0.2f, 25, Enums.WeaponType.RIFLE)
		{
		}
	}
}