using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Smg
{
    public class SMGMk2 : WeaponItemBase
    {
        public override int Id => 202;

        public SMGMk2() : base("SMG MKII", 0x78A97CD0, Enums.WeaponType.SMG, Enums.InjuryType.SHOT_HIGH)
        {
        }

    }
}
