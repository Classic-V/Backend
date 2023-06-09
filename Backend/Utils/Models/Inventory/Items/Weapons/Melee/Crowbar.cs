using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Melee
{
    public class Crowbar : WeaponItemBase
    {
        public override int Id => 183;
        public Crowbar() : base("Crowbar", 0x84BD7BFD, Enums.WeaponType.MEELE, Enums.InjuryType.PUNCH) {}
    }
}
