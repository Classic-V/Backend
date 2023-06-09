using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Melee
{
    public class Dagger : WeaponItemBase
    {
        public override int Id => 180;
        public Dagger() : base("Dagger", 0x92A27487, Enums.WeaponType.MEELE, Enums.InjuryType.SLICE) {}
    }
}
