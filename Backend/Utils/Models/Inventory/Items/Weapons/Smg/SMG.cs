using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Smg
{
    public class SMG : WeaponItemBase
    {
        public override int Id => 201;

        public SMG() : base("SMG", 0x2BE6766B, Enums.WeaponType.SMG, Enums.InjuryType.SHOT_HIGH)
        {
        }

    }
}
