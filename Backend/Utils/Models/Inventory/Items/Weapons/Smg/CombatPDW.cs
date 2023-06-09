using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Smg
{
    public class CombatPDW : WeaponItemBase
    {
        public override int Id => 204;

        public CombatPDW() : base("Combat PDW", 0x0A3D4D34, Enums.WeaponType.SMG, Enums.InjuryType.SHOT_HIGH)
        {
        }

    }
}
