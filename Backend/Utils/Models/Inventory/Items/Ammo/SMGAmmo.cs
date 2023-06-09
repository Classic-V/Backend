using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Ammo
{
	public class SMGAmmo : AmmoItemBase
	{
		public override int Id => 2002;

		public SMGAmmo() : base("SMG Munition", 0.2f, 25, Enums.WeaponType.SMG)
		{
		}
	}
}