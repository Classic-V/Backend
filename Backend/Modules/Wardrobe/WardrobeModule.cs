using AltV.Net.Elements.Entities;
using Backend.Controllers.Event.Interface;
using Backend.Controllers.Outfit.Interface;
using Backend.Controllers.Wardrobe.Interface;
using Backend.Data;
using Backend.Utils.Enums;
using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Input;
using Backend.Utils.Models.Player.Client;
using Newtonsoft.Json;
using Backend.Utils.Models.ClothesShop;
using AltV.Net;

namespace Backend.Modules.Wardrobe;

public class WardrobeModule : Module<WardrobeModule>, IEventColshape
{
    private readonly IOutfitController _outfitController;
    private readonly IWardrobeController _wardrobeController;

    public WardrobeModule(IEventController eventController, IOutfitController outfitController, IWardrobeController wardrobeController) : base("Wardrobe")
    {
        _outfitController = outfitController;
        _wardrobeController = wardrobeController;

        eventController.OnClient("Server:Wardrobe:Open", Open);
        eventController.OnClient("Server:Wardrobe:ShowOutfit", ShowOutfit);
        eventController.OnClient<string>("Server:Wardrobe:ShowClothing", ShowClothing);

        eventController.OnClient<int>("Server:Wardrobe:SelectClothing", SelectClothing);
        eventController.OnClient<string>("Server:Wardrobe:SelectClothingEmpty", SelectClothingEmpty);

        eventController.OnClient<int>("Server:Wardrobe:ChooseOutfit", ChooseOutfit);
        eventController.OnClient<int>("Server:Wardrobe:SelectOutfit", SelectOutfit);
        eventController.OnClient<int>("Server:Wardrobe:DeleteOutfit", DeleteOutfit);

        eventController.OnClient<int, string>("Server:Wardrobe:SaveOutfit", SaveOutfit);
        eventController.OnClient("Server:Wardrobe:ChooseSaveOutfit", ChooseSaveOutfit);
    }

    private async void Open(ClPlayer player, string eventKey)
    {
        if (player.DbModel == null! || player.IsDead || player.Interaction || player.CurrentShape == null) return;
        
        var shape = player.CurrentShape;
        if (shape.ShapeType != ColshapeType.WARDROBE) return;

        var id = player.CurrentShape.Id;
        var wardrobe = await _wardrobeController.GetWardrobe(id);
        if (wardrobe == null) return;

        var items = new List<ClientNativeMenuItem>
        {
            new("Outfits", true, "Server:Wardrobe:ShowOutfit"),
            new("Maske", true, "Server:Wardrobe:ShowClothing", "Mask"),
            new("Körper", true, "Server:Wardrobe:ShowClothing", "Torsos"),
            new("Hose", true, "Server:Wardrobe:ShowClothing", "Legs"),
            new("Rucksäcke", true, "Server:Wardrobe:ShowClothing", "Bags"),
            new("Schuhe", true, "Server:Wardrobe:ShowClothing", "Shoes"),
            new("Accessories", true, "Server:Wardrobe:ShowClothing", "Accessories"),
            new("Unterteil", true, "Server:Wardrobe:ShowClothing", "Undershirt"),
            new("Makierungen", true, "Server:Wardrobe:ShowClothing", "Decals"),
            new("Oberteil", true, "Server:Wardrobe:ShowClothing", "Tops"),
            new("Hut", true, "Server:Wardrobe:ShowClothing", "Hats"),
            new("Brillen", true, "Server:Wardrobe:ShowClothing", "Glasses"),
            new("Ohren", true, "Server:Wardrobe:ShowClothing", "Ears"),
            new("Uhren", true, "Server:Wardrobe:ShowClothing", "Watches"),
            new("Ketten", true, "Server:Wardrobe:ShowClothing", "Bracelets"),
        };

        var menu = new ClientNativeMenu("Kleiderschrank", items);
        await player.ShowNativeMenu(true, menu);
    }

    private async void ShowOutfit(ClPlayer player, string eventKey)
    {
        var outfits = await _outfitController.GetOutfits(player.DbModel.Id);
        if (outfits == null) return;

        var list = new List<ClientNativeMenuItem>();
        list.Add(new ClientNativeMenuItem("Outfit speichern", true, "Server:Wardrobe:ChooseSaveOutfit"));

        outfits.ForEach(item =>
        {
            var outfit = new ClientNativeMenuItem(item.Name, true, "Server:Wardrobe:ChooseOutfit", item.Id);
            list.Add(outfit);
        });

        var menu = new ClientNativeMenu("Outfits", list);
        await player.ShowNativeMenu(true, menu);
    }

    private async void ShowClothing(ClPlayer player, string eventKey, string clothingItem)
    {
        var boughtItems = player.DbModel.WardrobeClothes;
        if (boughtItems == null!) return;

        var list = new List<ClientNativeMenuItem>();

        switch (clothingItem)
        {
            case "Mask":
                list.Add(new ClientNativeMenuItem("Leer", false, "Server:Wardrobe:SelectClothingEmpty", "mask"));
                boughtItems.Where(x => !x.IsProp && x.Component == 1).ToList().ForEach(item =>
                {
                    list.Add(new ClientNativeMenuItem(item.Name, false, "Server:Wardrobe:SelectClothing", item.Id));
                });
                break;
            case "Tops":
                list.Add(new ClientNativeMenuItem("Leer", false, "Server:Wardrobe:SelectClothingEmpty", "tops"));
                boughtItems.Where(x => !x.IsProp && x.Component == 11).ToList().ForEach(item =>
                {
                    list.Add(new ClientNativeMenuItem(item.Name, false, "Server:Wardrobe:SelectClothing", item.Id));
                });
                break;
            case "Torsos":
                boughtItems.Where(x => !x.IsProp && x.Component == 3).ToList().ForEach(item =>
                {
                    list.Add(new ClientNativeMenuItem(item.Name, false, "Server:Wardrobe:SelectClothing", item.Id));
                }); 
                break;
            case "Legs":
                list.Add(new ClientNativeMenuItem("Leer", false, "Server:Wardrobe:SelectClothingEmpty", "legs"));
                boughtItems.Where(x => !x.IsProp && x.Component == 4).ToList().ForEach(item =>
                {
                    list.Add(new ClientNativeMenuItem(item.Name, false, "Server:Wardrobe:SelectClothing", item.Id));
                });
                break;
            case "Bags":
                list.Add(new ClientNativeMenuItem("Leer", false, "Server:Wardrobe:SelectClothingEmpty", "bags"));
                boughtItems.Where(x => !x.IsProp && x.Component == 5).ToList().ForEach(item =>
                {
                    list.Add(new ClientNativeMenuItem(item.Name, false, "Server:Wardrobe:SelectClothing", item.Id));
                });
                break;
            case "Shoes":
                list.Add(new ClientNativeMenuItem("Leer", false, "Server:Wardrobe:SelectClothingEmpty", "shoes"));
                boughtItems.Where(x => !x.IsProp && x.Component == 6).ToList().ForEach(item =>
                {
                    list.Add(new ClientNativeMenuItem(item.Name, false, "Server:Wardrobe:SelectClothing", item.Id));
                });
                break;
            case "Accessories":
                list.Add(new ClientNativeMenuItem("Leer", false, "Server:Wardrobe:SelectClothingEmpty", "accessories"));
                boughtItems.Where(x => !x.IsProp && x.Component == 7).ToList().ForEach(item =>
                {
                    list.Add(new ClientNativeMenuItem(item.Name, false, "Server:Wardrobe:SelectClothing", item.Id));
                });
                break;
            case "Undershirt":
                list.Add(new ClientNativeMenuItem("Leer", false, "Server:Wardrobe:SelectClothingEmpty", "undershirt"));
                boughtItems.Where(x => !x.IsProp && x.Component == 8).ToList().ForEach(item =>
                {
                    list.Add(new ClientNativeMenuItem(item.Name, false, "Server:Wardrobe:SelectClothing", item.Id));
                });
                break;
            case "Decals":
                list.Add(new ClientNativeMenuItem("Leer", false, "Server:Wardrobe:SelectClothingEmpty", "decals"));
                boughtItems.Where(x => !x.IsProp && x.Component == 10).ToList().ForEach(item =>
                {
                    list.Add(new ClientNativeMenuItem(item.Name, false, "Server:Wardrobe:SelectClothing", item.Id));
                });
                break;
            case "Hats":
                list.Add(new ClientNativeMenuItem("Leer", false, "Server:Wardrobe:SelectClothingEmpty", "hats"));
                boughtItems.Where(x => x.IsProp && x.Component == 0).ToList().ForEach(item =>
                {
                    list.Add(new ClientNativeMenuItem(item.Name, false, "Server:Wardrobe:SelectClothing", item.Id));
                });
                break;
            case "Glasses":
                list.Add(new ClientNativeMenuItem("Leer", false, "Server:Wardrobe:SelectClothingEmpty", "glasses"));
                boughtItems.Where(x => x.IsProp && x.Component == 1).ToList().ForEach(item =>
                {
                    list.Add(new ClientNativeMenuItem(item.Name, false, "Server:Wardrobe:SelectClothing", item.Id));
                });
                break;
            case "Ears":
                list.Add(new ClientNativeMenuItem("Leer", false, "Server:Wardrobe:SelectClothingEmpty", "ears"));
                boughtItems.Where(x => x.IsProp && x.Component == 2).ToList().ForEach(item =>
                {
                    list.Add(new ClientNativeMenuItem(item.Name, false, "Server:Wardrobe:SelectClothing", item.Id));
                });
                break;
            case "Watches":
                list.Add(new ClientNativeMenuItem("Leer", false, "Server:Wardrobe:SelectClothingEmpty", "watches"));
                boughtItems.Where(x => x.IsProp && x.Component == 6).ToList().ForEach(item =>
                {
                    list.Add(new ClientNativeMenuItem(item.Name, false, "Server:Wardrobe:SelectClothing", item.Id));
                });
                break;
            case "Bracelets":
                list.Add(new ClientNativeMenuItem("Leer", false, "Server:Wardrobe:SelectClothingEmpty", "bracelets"));
                boughtItems.Where(x => x.IsProp && x.Component == 7).ToList().ForEach(item =>
                {
                    list.Add(new ClientNativeMenuItem(item.Name, false, "Server:Wardrobe:SelectClothing", item.Id));
                });
                break;
            default: break;
        }

        var menu = new ClientNativeMenu("Kleidung", list);
        await player.ShowNativeMenu(true, menu);

    }

    private async void SelectClothing(ClPlayer player, string eventKey, int clothingId)
    {
        var item = player.DbModel.WardrobeClothes.FirstOrDefault(x => x.Id == clothingId);
        if (item == null) return;

		var hash = Alt.Hash(item.Dlc);

		if (item.IsProp)
        {
			switch (item.Component)
            {
                case 0:
                    player.DbModel.Clothes.Hat = new ClothingModel(item.Component, item.Drawable, item.Texture, hash);
                    break;
                case 1:
                    player.DbModel.Clothes.Glasses =  new ClothingModel(item.Component, item.Drawable, item.Texture, hash);
                    break;
                case 2:
                    player.DbModel.Clothes.Ears =  new ClothingModel(item.Component, item.Drawable, item.Texture, hash);
                    break;
                case 6:
                    player.DbModel.Clothes.Watch =  new ClothingModel(item.Component, item.Drawable, item.Texture, hash);
                    break;
                case 7:
                    player.DbModel.Clothes.Bracelet =  new ClothingModel(item.Component, item.Drawable, item.Texture, hash);
                    break;
            }
        }
        else
        {
            switch (item.Component)
            {
                case 1:
                    player.DbModel.Clothes.Mask =  new ClothingModel(item.Component, item.Drawable, item.Texture, hash);
                    break;
                case 3:
                    player.DbModel.Clothes.Body =  new ClothingModel(item.Component, item.Drawable, item.Texture, hash);
                    break;
                case 4:
                    player.DbModel.Clothes.Leg =  new ClothingModel(item.Component, item.Drawable, item.Texture, hash);
                    break;
                case 5:
                    player.DbModel.Clothes.Bag =  new ClothingModel(item.Component, item.Drawable, item.Texture, hash);
                    break;
                case 6:
                    player.DbModel.Clothes.Shoe =  new ClothingModel(item.Component, item.Drawable, item.Texture, hash);
                    break;
                case 7:
                    player.DbModel.Clothes.Accessories =  new ClothingModel(item.Component, item.Drawable, item.Texture, hash);
                    break;
                case 8:
                    player.DbModel.Clothes.Undershirt =  new ClothingModel(item.Component, item.Drawable, item.Texture, hash);
                    break;
                case 10:
                    player.DbModel.Clothes.Decals =  new ClothingModel(item.Component, item.Drawable, item.Texture, hash);
                    break;
                case 11:
                    player.DbModel.Clothes.Top =  new ClothingModel(item.Component, item.Drawable, item.Texture, hash);
                    break;
            }
        }

        await player.ApplyClothes();
    }

    private async void SelectClothingEmpty(ClPlayer player, string eventKey, string clothing)
    {
        switch (clothing)
        {
            case "mask":
				player.DbModel.Clothes.Mask = new ClothingModel(1, 0, 0, 0);
				break;
            case "legs":
				player.DbModel.Clothes.Leg = new ClothingModel(4, 21, 0, 0);
                break;
            case "bags":
				player.DbModel.Clothes.Bag = new ClothingModel(5, 0, 0, 0);
				break;
            case "shoes":
				player.DbModel.Clothes.Shoe = new ClothingModel(6, 34, 0, 0);
				break;
            case "accessories":
				player.DbModel.Clothes.Accessories = new ClothingModel(7, 0, 0, 0);
				break;
            case "undershirt":
				player.DbModel.Clothes.Undershirt = new ClothingModel(8, 15, 0, 0);
				break;
            case "decals":
				player.DbModel.Clothes.Decals = new ClothingModel(10, 0, 0, 0);
				break;
            case "tops":
				player.DbModel.Clothes.Top = new ClothingModel(11, 15, 0, 0);
				break;
            case "hats":
				player.DbModel.Clothes.Hat = new ClothingModel(0, -1, 0, 0);
				break;
            case "glasses":
				player.DbModel.Clothes.Glasses = new ClothingModel(1, -1, 0, 0);
				break;
            case "ears":
				player.DbModel.Clothes.Ears = new ClothingModel(2, -1, 0, 0);
				break;
            case "watches":
				player.DbModel.Clothes.Watch = new ClothingModel(6, -1, 0, 0);
				break;
            case "bracelets":
				player.DbModel.Clothes.Bracelet = new ClothingModel(7, -1, 0, 0);
				break;
            default: break;
        }

        await player.ApplyClothes();
    }

    private async void ChooseSaveOutfit(ClPlayer player, string eventKey)
    {
        await player.ShowComponent("Input", true,
            JsonConvert.SerializeObject(new InputDataModel("Outfit speichern", "Gebe dein Outfit Namen an",
                InputType.TEXT, "Server:Wardrobe:SaveOutfit", player.DbModel.Id)));
    }

    private async void SaveOutfit(ClPlayer player, string eventKey, int playerId, string outfitName)
    {
        var outfit = new OutfitModel(playerId, outfitName, player.DbModel.Clothes);

        await _outfitController.CreateOutfit(outfit);
        await player.Notify("Kleiderschrank", "Outfit erfolgreich gespeichert.", NotificationType.SUCCESS);
    }

    private async void ChooseOutfit(ClPlayer player, string eventKey, int outfitId)
    {
        var outfit = await _outfitController.GetOutfit(outfitId);
        if (outfit == null) return;

        var items = new List<ClientNativeMenuItem>
        {
            new("Outfit anziehen", true, "Server:Wardrobe:SelectOutfit", outfit.Id),
            new("Outfit löschen", true, "Server:Wardrobe:DeleteOutfit", outfit.Id)
        };

        var menu = new ClientNativeMenu(outfit.Name, items);
        await player.ShowNativeMenu(true, menu);
    }

    private async void SelectOutfit(ClPlayer player, string eventKey, int outfitId)
    {
        var outfit = await _outfitController.GetOutfit(outfitId);
        if (outfit == null) return;

        player.DbModel.Clothes.Mask = outfit.Masks;
        player.DbModel.Clothes.Top = outfit.Tops;
        player.DbModel.Clothes.Undershirt = outfit.Undershirts;
        player.DbModel.Clothes.Body = outfit.Torsos;
        player.DbModel.Clothes.Leg = outfit.Legs;
        player.DbModel.Clothes.Shoe = outfit.Shoes;
        player.DbModel.Clothes.Bag = outfit.Bags;
        player.DbModel.Clothes.Accessories = outfit.Accessories;
        player.DbModel.Clothes.Hat = outfit.Hats;
        player.DbModel.Clothes.Glasses = outfit.Glasses;
        player.DbModel.Clothes.Ears = outfit.Ears;
        player.DbModel.Clothes.Watch = outfit.Watches;
        player.DbModel.Clothes.Bracelet = outfit.Bracelets;

        await player.ApplyClothes();
        await player.Notify("Kleiderschrank", "Outfit erfolgreich angezogen.", NotificationType.SUCCESS);
    }

    private async void DeleteOutfit(ClPlayer player, string eventKey, int outfitId)
    {
        var outfit = await _outfitController.GetOutfit(outfitId);
        if (outfit == null) return;

        await _outfitController.DeleteOutfit(outfit);
        await player.Notify("Kleiderschrank", "Outfit erfolgreich gelöscht.", NotificationType.SUCCESS);
    }

    public async Task OnColshape(ClShape shape, IEntity entity, bool entered)
    {
        if (entity.Type != BaseObjectType.Player) return;

        ClPlayer player = (ClPlayer)entity;
        if (player == null!) return;
        if (!entered) return;
        if (shape.ShapeType != ColshapeType.WARDROBE) return;

        player.SetInteraction(Interactions.KEY_E, entered ? Interactions.E_WARDROBE : Interactions.NONE);
    }
}