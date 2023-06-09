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
    public class Gadgetpistol : WeaponItemBase
    {
        public override int Id => 147;

        public Gadgetpistol() : base("Gadget Pistol", 0x57A4368C, Enums.WeaponType.PISTOL, Enums.InjuryType.SHOT_LOW)
        {
        }
    }
}