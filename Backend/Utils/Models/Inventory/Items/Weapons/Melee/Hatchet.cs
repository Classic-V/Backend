using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Melee
{
    public class Hatchet : WeaponItemBase
    {
        public override int Id => 187;
        public Hatchet() : base("Hatchet", 0xF9DCBF2D, Enums.WeaponType.MEELE, Enums.InjuryType.SLICE) {}
    }
}
