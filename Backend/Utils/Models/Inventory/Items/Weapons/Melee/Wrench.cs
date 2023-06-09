
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Melee
{
    public class Wrench : WeaponItemBase
    {
        public override int Id => 193;
        public Wrench() : base("Wrench", 0x19044EE0, Enums.WeaponType.MEELE, Enums.InjuryType.PUNCH) {}
    }
}
