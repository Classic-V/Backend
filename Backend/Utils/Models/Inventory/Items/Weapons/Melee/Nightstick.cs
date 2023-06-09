
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Melee
{
    public class Nightstick : WeaponItemBase
    {
        public override int Id => 192;
        public Nightstick() : base("Nightstick", 0x678B81B1, Enums.WeaponType.MEELE, Enums.InjuryType.PUNCH) {}
    }
}
