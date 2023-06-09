
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Melee
{
    public class StoneHatchet : WeaponItemBase
    {
        public override int Id => 195;
        public StoneHatchet() : base("StoneHatchet", 0x3813FC08, Enums.WeaponType.MEELE, Enums.InjuryType.SLICE) {}
    }
}
