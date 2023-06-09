using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Smg
{
    public class MicroSmg : WeaponItemBase
    {
        public override int Id => 200;

        public MicroSmg() : base("Micro SMG", 0x13532244, Enums.WeaponType.SMG, Enums.InjuryType.SHOT_LOW)
        {
        }

    }
}
