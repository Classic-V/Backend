using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Backend.Controllers.Shop.Interface;
using Backend.Services.Shop.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Shops;
using Backend.Utils.Streamer;

namespace Backend.Controllers.Shop
{
    public class ShopController : IShopController
    {
        private IShopService _shopService;

        public ShopController(IShopService shopService)
        {
            _shopService = shopService;

            _shopService.Shops.ForEach(LoadShop);
        }

        public void LoadShop(ShopModel model)
        {
            if(model.Type == ShopType.STANDARD)
            {
				var blip = Alt.CreateBlip(BlipType.Destination, model.Position);
				blip.Dimension = 0;
				blip.Sprite = 52;
				blip.Name = "Shop";
				blip.ShortRange = true;
				blip.Color = 2;
			}

            var colShape = (ClShape)Alt.CreateColShapeSphere(model.Position, 1f);
            colShape.Id = model.Id;
			colShape.ShapeType = ColshapeType.SHOP;
			colShape.Size = 1f;
			colShape.Dimension = 0;

			PedStreamer.AddObject(new Ped(model.Type == ShopType.STANDARD ? 0x18CE57D0 : model.Type == ShopType.TEAM ? (uint)PedModel.Cop01SMY : 0x8CDCC057, model.PedPosition.X, model.PedPosition.Y, model.PedPosition.Z, model.PedPosition.H, true, true, 0));
		}

        public async Task<int> CreateShop(ShopModel model)
        {
            await _shopService.AddShop(model);

            return model.Id;
        }

        public async Task<ShopModel?> GetShop(int id)
        {
            return await _shopService.GetShop(id);
        }

        public async Task<ShopItemModel?> GetItemData(int shopId, int id)
        {
            var shop = await GetShop(shopId);
            if (shop == null) return null;

            return shop.Items.FirstOrDefault(x => x.ItemId == id)!;
        }

        public async Task SaveShops()
        {
            for(var i = 0; i < _shopService.Shops.Count; i++)
                await _shopService.UpdateShop(_shopService.Shops[i]);
        }
    }
}
