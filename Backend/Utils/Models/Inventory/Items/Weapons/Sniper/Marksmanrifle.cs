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
    public class Marksmanrifle : WeaponItemBase
    {
        public override int Id => 172;

        public Marksmanrifle() : base("Marksmanrifle", 0xC734385A, Enums.WeaponType.SNIPER, Enums.InjuryType.SHOT_HIGH)
        {
        }
    }
}