
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Melee
{
    public class Machete : WeaponItemBase
    {
        public override int Id => 190;
        public Machete() : base("Machete", 0xDD5DF8D9, Enums.WeaponType.MEELE, Enums.InjuryType.SLICE) {}
    }
}
