using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Smg
{
    public class MachinePistol : WeaponItemBase
    {
        public override int Id => 205;

        public MachinePistol() : base("Machine Pistol", 0xDB1AA450, Enums.WeaponType.SMG, Enums.InjuryType.SHOT_LOW)
        {
        }

    }
}
