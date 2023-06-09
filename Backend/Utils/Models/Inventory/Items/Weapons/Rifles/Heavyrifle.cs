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
    public class Heavyrifle : WeaponItemBase
    {
        public override int Id => 158;

        public Heavyrifle() : base("Heavyrifle", 0xC78D71B4, Enums.WeaponType.RIFLE, Enums.InjuryType.SHOT_HIGH)
        {
        }
    }
}