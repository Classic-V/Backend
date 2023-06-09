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
    public class Sniperrifle : WeaponItemBase
    {
        public override int Id => 175;

        public Sniperrifle() : base("Sniperrifle", 0x5FC3C11, Enums.WeaponType.SNIPER, Enums.InjuryType.SHOT_HIGH)
        {
        }
    }
}