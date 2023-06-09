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
    public class RevolverMk2 : WeaponItemBase
    {
        public override int Id => 139;

        public RevolverMk2() : base("Revolver MKII", 0xCB96392F, Enums.WeaponType.PISTOL, Enums.InjuryType.SHOT_LOW)
        {
        }
    }
}