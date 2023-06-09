using AltV.Net.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Rifles
{
    public class SpecialcarbineMk2 : WeaponItemBase
    {
        public override int Id => 161;

        public SpecialcarbineMk2() : base("Specialcarbine MKII", 2526821735, Enums.WeaponType.RIFLE, Enums.InjuryType.SHOT_HIGH)
        {
        }
    }
}