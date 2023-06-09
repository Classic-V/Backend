using AltV.Net.Enums;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Pistols
{
    public class Pistol50 : WeaponItemBase
    {
        public override int Id => 135;

        public Pistol50() : base("Pistol50", 0x99AEEB3B, Enums.WeaponType.PISTOL, Enums.InjuryType.SHOT_LOW)
        {
        }
    }
}