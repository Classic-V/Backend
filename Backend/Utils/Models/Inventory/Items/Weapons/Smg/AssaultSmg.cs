using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Smg
{
    public class AssaultSmg : WeaponItemBase
    {
        public override int Id => 203;

        public AssaultSmg() : base("Assault Smg", 0xEFE7E2DF, Enums.WeaponType.SMG, Enums.InjuryType.SHOT_HIGH)
        {
        }

    }
}
