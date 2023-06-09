
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Melee
{
    public class Switchblade : WeaponItemBase
    {
        public override int Id => 191;
        public Switchblade() : base("Switchblade", 0xDFE37640, Enums.WeaponType.MEELE, Enums.InjuryType.SLICE) {}
    }
}
