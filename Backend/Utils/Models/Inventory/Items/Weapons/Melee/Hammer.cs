using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Melee
{
    public class Hammer : WeaponItemBase
    {
        public override int Id => 186;
        public Hammer() : base("Hammer", 0x4E875F73, Enums.WeaponType.MEELE, Enums.InjuryType.PUNCH) {}
    }
}
