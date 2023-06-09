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
    public class PistolMk2 : WeaponItemBase
    {
        public override int Id => 136;

        public PistolMk2() : base("Pistol MKII", 0xBFE256D4, Enums.WeaponType.PISTOL, Enums.InjuryType.SHOT_LOW)
        {
        }
    }
}