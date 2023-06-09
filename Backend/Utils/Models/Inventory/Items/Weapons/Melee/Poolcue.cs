
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Melee
{
    public class Poolcue : WeaponItemBase
    {
        public override int Id => 195;
        public Poolcue() : base("Poolcue", 0x94117305, Enums.WeaponType.MEELE, Enums.InjuryType.PUNCH) {}
    }
}
