using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Smg
{
    public class MiniSMG : WeaponItemBase
    {
        public override int Id => 206;

        public MiniSMG() : base("Mini SMG", 0xBD248B55, Enums.WeaponType.SMG, Enums.InjuryType.SHOT_LOW)
        {
        }

    }
}
