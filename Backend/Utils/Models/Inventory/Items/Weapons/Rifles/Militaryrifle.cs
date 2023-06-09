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
    public class Militaryrifle : WeaponItemBase
    {
        public override int Id => 159;

        public Militaryrifle() : base("Militaryrifle", 0x9D1F17E6, Enums.WeaponType.RIFLE, Enums.InjuryType.SHOT_HIGH)
        {
        }
    }
}