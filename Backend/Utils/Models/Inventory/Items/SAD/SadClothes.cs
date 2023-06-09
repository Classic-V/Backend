using AltV.Net;
using Backend.Modules.Inventory;
using Backend.Utils.Enums;
using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items.SAD
{
    public class SadClothes : ItemBase
    {
        public override int Id => 9991;

        public SadClothes() : base("SAD Einsatzkleidung", 0.2f, 1)
        {
        }

        public override async Task<bool> Use(ClPlayer player, int slot, int amount)
        {
            if (player.DbModel.PlayerCachedClothes != null || player.DbModel.Team != 2 || player.DbModel.TeamRank < 4 || !player.DbModel.SadDuty) return false;

            player.PlayAnimation(AnimationType.USE_VEST);
            await player.StartInteraction(() =>
            {
                var dlc = Alt.Hash("mp_m_fib");

                player.DbModel.PlayerCachedClothes = player.DbModel.Clothes;
                player.DbModel.Clothes = new(
                    new(1, 125, 0, 0),
                    new(11, 89, 0, 0),
                    new(8, 130, 0, 0),
                    new(3, 17, 0, 0),
                    new(4, 33, 0, 0),
                    new(6, 112, 0, 0),
                    player.DbModel.Clothes.Armor,
                    new(5, 0, 0, 0),
                    new(7, 0, 0, dlc),

                    new(0, 119, 0, 0),
                    new(1, 25, 0, 0),
                    new(2, -1, 0, 0),
                    new(6, -1, 0, 0),
                    new(7, -1, 0, 0));

                player.ApplyClothes();
                player.DbModel.Inventory.RemoveItem(slot, 1);
                player.DbModel.Inventory.AddItem(InventoryModule.GetItemBase(31)!, 1);
            }, 5000);

            return true;
        }
    }
}
