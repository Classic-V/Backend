
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Melee
{
    public class BattleAxe : WeaponItemBase
    {
        public override int Id => 194;
        public BattleAxe() : base("BattleAxe", 0xCD274149, Enums.WeaponType.MEELE, Enums.InjuryType.SLICE) {}
    }
}
