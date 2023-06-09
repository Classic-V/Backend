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
    public class Precisionrifle : WeaponItemBase
    {
        public override int Id => 174;

        public Precisionrifle() : base("Precisionrifle", 0x6E7DDDEC, Enums.WeaponType.SNIPER, Enums.InjuryType.SHOT_HIGH)
        {
        }
    }
}