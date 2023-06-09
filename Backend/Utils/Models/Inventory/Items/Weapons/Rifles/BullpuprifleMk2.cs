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
    public class BullpuprifleMk2 : WeaponItemBase
    {
        public override int Id => 154;

        public BullpuprifleMk2() : base("Bullpuprifle MKII", 0x84D6FAFD, Enums.WeaponType.RIFLE, Enums.InjuryType.SHOT_HIGH)
        {
        }
    }
}