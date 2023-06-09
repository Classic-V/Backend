using AltV.Net;
using Backend.Utils.Enums;
using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.SAD
{
    public class SadArmor : ItemBase
    {
        public override int Id => 9999;

        public SadArmor() : base("SAD Schutzweste", 1.5f, 10)
        {
        }

        public override async Task<bool> Use(ClPlayer player, int slot, int amount)
        {
            if (player.DbModel.Team != 2 || player.DbModel.TeamRank < 4 || !player.DbModel.SadDuty) return false;

            player.PlayAnimation(AnimationType.USE_VEST);
            await player.StartInteraction(() =>
            {
                var hash = Alt.Hash("mp_m_frakclothesm");
                player.SetHealth(player.Health, 100);
                player.DbModel.Clothes.Armor.Drawable = 2;
                player.DbModel.Clothes.Armor.Texture = 7;
                player.DbModel.Clothes.Armor.Dlc = hash;
                player.SetDlcClothes(9, 2, 7, 0, hash);
                player.DbModel.Inventory.RemoveItem(slot, 1);
            }, 4000);

            return true;
        }
    }
}
