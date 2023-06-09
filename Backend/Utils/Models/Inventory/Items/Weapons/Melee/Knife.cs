using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Melee
{
    public class Knife : WeaponItemBase
    {
        public override int Id => 189;
        public Knife() : base("Knife", 0x99B507EA, Enums.WeaponType.MEELE, Enums.InjuryType.SLICE) {}
    }
}
