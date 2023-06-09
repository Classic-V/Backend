using Backend.Controllers.Event.Interface;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Player.Client;
using Newtonsoft.Json;

namespace Backend.Modules.ClothingMenu;

public class ClothingMenuModule : Module<ClothingMenuModule>
{
    public ClothingMenuModule(IEventController eventController) : base("ClothingMenu")
    {
        eventController.OnClient("Server:ClothesMenu:Open", OpenMenu);
        eventController.OnClient<string, bool>("Server:ClothesMenu:TakeOff", TakeOffClothing);
    }

    private void OpenMenu(ClPlayer player, string eventKey)
    {
        if (player.DbModel == null || player.IsDead || player.Interaction) return;

        player.ShowComponent("ClothesMenu", true, JsonConvert.SerializeObject(new
        {
            Mask = player.GetClothes(1).Drawable == 0,
            Top = player.GetClothes(11).Drawable == 15,
            Pants = player.GetClothes(4).Drawable == 21,
            Shoes = player.GetClothes(6).Drawable == 34,
            Accessories = player.GetClothes(7).Drawable == 0,
            Hat = player.GetProps(0).Drawable == 8,
            Glasses = player.GetProps(1).Drawable == 0,
            Watch = player.GetProps(6).Drawable == 2,
            Ears = player.GetProps(2).Drawable == 3,
            Bracelet = player.GetProps(7).Drawable == 99
		}));
    }

    private void TakeOffClothing(ClPlayer player, string eventKey, string clothing, bool state)
    {
        if (player.DbModel == null) return;

        switch (clothing)
        {
            case "Maske":
                if (state)
                {
                    player?.SetClothing(1, player.DbModel.Clothes.Mask.Drawable, player.DbModel.Clothes.Mask.Texture, player.DbModel.Clothes.Mask.Dlc);
                }
                else
                {
                    player?.SetClothing(1, 0, 0, 0);
                }
                break;
            case "Brille":
                if (state)
                {
                    player?.SetProp(1, player.DbModel.Clothes.Glasses.Drawable, player.DbModel.Clothes.Glasses.Texture, player.DbModel.Clothes.Glasses.Dlc);
                }
                else
                {
                    player?.SetProp(1, 0, 0, 0);
                }
                break;
            case "Uhren":
                if (state)
                {
                    player?.SetProp(6, player.DbModel.Clothes.Watch.Drawable,
                        player.DbModel.Clothes.Watch.Texture, player.DbModel.Clothes.Watch.Dlc);
                }
                else
                {
                    player?.SetProp(6, 2, 0, 0);
                }
                break;
            case "Ohrringe":
                if (state)
                {
                    player?.SetProp(2, player.DbModel.Clothes.Ears.Drawable,
                        player.DbModel.Clothes.Ears.Texture, player.DbModel.Clothes.Ears.Dlc);
                }
                else
                {
                    player?.SetProp(2, 3, 0, 0);
                }
                break;
            case "Armband":
                if (state)
                {
                    player?.SetProp(7, player.DbModel.Clothes.Bracelet.Drawable,
                        player.DbModel.Clothes.Bracelet.Texture, player.DbModel.Clothes.Bracelet.Dlc);
                }
                else
                {
                    player?.SetProp(7, 99, 0, 0);
                }
                break;
            case "Hut":
                if (state)
                {
                    player?.SetProp(0, player.DbModel.Clothes.Hat.Drawable,
                        player.DbModel.Clothes.Hat.Texture, player.DbModel.Clothes.Hat.Dlc);
                }
                else
                {
                    player?.SetProp(0, 8, 0, 0);
                }
                break;
            case "Oberteil":
                if (state)
                {
                    player?.SetClothing(11, player.DbModel.Clothes.Top.Drawable,
                        player.DbModel.Clothes.Top.Texture, player.DbModel.Clothes.Top.Dlc);
                    player?.SetClothing(8, player.DbModel.Clothes.Undershirt.Drawable,
                        player.DbModel.Clothes.Undershirt.Texture, player.DbModel.Clothes.Undershirt.Dlc);
                    player?.SetClothing(3, player.DbModel.Clothes.Body.Drawable,
                        player.DbModel.Clothes.Body.Texture, player.DbModel.Clothes.Body.Dlc);
                }
                else
                {
                    player?.SetClothing(11, 15, 0, 0);
                    player?.SetClothing(8, 15, 0, 0);
                    player?.SetClothing(3, 15, 0, 0);
                }
                break;
            case "Hose":
                if (state)
                {
                    player?.SetClothing(4, player.DbModel.Clothes.Leg.Drawable,
                        player.DbModel.Clothes.Leg.Texture, player.DbModel.Clothes.Leg.Dlc);
                }
                else
                {
                    player?.SetClothing(4, 21, 0, 0);
                }
                break;
            case "Schuhe":
                if (state)
                {
                    player?.SetClothing(6, player.DbModel.Clothes.Shoe.Drawable,
                        player.DbModel.Clothes.Shoe.Texture, player.DbModel.Clothes.Shoe.Dlc);
                }
                else
                {
                    player?.SetClothing(6, 34, 0, 0);
                }
                break;
            case "Accessories":
				if (state)
				{
					player?.SetClothing(7, player.DbModel.Clothes.Accessories.Drawable,
						player.DbModel.Clothes.Accessories.Texture, player.DbModel.Clothes.Accessories.Dlc);
				}
				else
				{
					player?.SetClothing(7, 0, 0, 0);
				}
				break;
            default: break;
        }
    }
}