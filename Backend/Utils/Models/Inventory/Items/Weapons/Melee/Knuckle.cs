using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Melee
{
    public class Knuckle : WeaponItemBase
    {
        public override int Id => 188;
        public Knuckle() : base("Knuckle", 0xD8DF3C3C, Enums.WeaponType.MEELE, Enums.InjuryType.PUNCH) {}
    }
}
