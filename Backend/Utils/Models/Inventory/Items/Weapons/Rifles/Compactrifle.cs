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
    public class Compactrifle : WeaponItemBase
    {
        public override int Id => 157;

        public Compactrifle() : base("Compactrifle", 0x624FE830, Enums.WeaponType.RIFLE, Enums.InjuryType.SHOT_HIGH)
        {
        }
    }
}