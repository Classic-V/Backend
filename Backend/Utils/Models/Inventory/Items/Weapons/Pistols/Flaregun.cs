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
    public class Flaregun : WeaponItemBase
    {
        public override int Id => 132;

        public Flaregun() : base("Flaregun", 0x47757124, Enums.WeaponType.PISTOL, Enums.InjuryType.FIRE)
        {
        }
    }
}