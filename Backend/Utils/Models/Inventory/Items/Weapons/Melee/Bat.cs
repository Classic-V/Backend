using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Melee
{
    public class Bat : WeaponItemBase
    {
        public override int Id => 181;
        public Bat() : base("Baseball Schläger", 0x958A4A8F, Enums.WeaponType.MEELE, Enums.InjuryType.PUNCH) {}
    }
}
