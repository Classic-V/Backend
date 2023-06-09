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
    public class CarbinerifleMk2 : WeaponItemBase
    {
        public override int Id => 156;

        public CarbinerifleMk2() : base("Carbinerifle MKII", 0xFAD1F1C9, Enums.WeaponType.RIFLE, Enums.InjuryType.SHOT_HIGH)
        {
        }
    }
}