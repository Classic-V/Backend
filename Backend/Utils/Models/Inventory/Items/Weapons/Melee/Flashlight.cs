using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Melee
{
    public class Flashlight : WeaponItemBase
    {
        public override int Id => 184;
        public Flashlight() : base("Flashlight", 0x8BB05FD7, Enums.WeaponType.MEELE, Enums.InjuryType.PUNCH) {}
    }
}
