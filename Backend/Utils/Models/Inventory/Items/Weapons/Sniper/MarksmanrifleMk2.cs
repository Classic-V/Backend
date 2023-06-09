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
    public class MarksmanrifleMk2 : WeaponItemBase
    {
        public override int Id => 173;

        public MarksmanrifleMk2() : base("Marksmanrifle MKII", 0x6A6C02E0, Enums.WeaponType.SNIPER, Enums.InjuryType.SHOT_HIGH)
        {
        }
    }
}