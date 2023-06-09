using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Melee
{
    public class Golfclub : WeaponItemBase
    {
        public override int Id => 185;
        public Golfclub() : base("Golfclub", 0x440E4788, Enums.WeaponType.MEELE, Enums.InjuryType.PUNCH) {}
    }
}
