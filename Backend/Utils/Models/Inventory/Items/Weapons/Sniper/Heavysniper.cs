using AltV.Net.Enums;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Sniper
{
    public class Heavysniper : WeaponItemBase
    {
        public override int Id => 170;

        public Heavysniper() : base("Heavysniper", 0xC472FE2, Enums.WeaponType.SNIPER, Enums.InjuryType.SHOT_HIGH)
        {
        }
    }
}