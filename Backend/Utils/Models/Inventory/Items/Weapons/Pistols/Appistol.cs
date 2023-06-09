﻿using AltV.Net.Enums;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.Weapons.Pistols
{
    public class Appistol : WeaponItemBase
    {
        public override int Id => 130;

        public Appistol() : base("AP Pistol", 0x22D8FE39, Enums.WeaponType.PISTOL, Enums.InjuryType.SHOT_LOW)
        {
        }
    }
}