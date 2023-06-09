using AltV.Net.Enums;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Rifles
{
    public class Specialcarbine : WeaponItemBase
    {
        public override int Id => 160;

        public Specialcarbine() : base("Specialcarbine", 3231910285, Enums.WeaponType.RIFLE, Enums.InjuryType.SHOT_HIGH)
        {
		}
    }
}