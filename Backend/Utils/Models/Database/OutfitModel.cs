using Backend.Utils.Models.ClothesShop;
using Backend.Utils.Models.Player;

namespace Backend.Utils.Models.Database;

public class OutfitModel
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public string Name { get; set; }
    public ClothingModel Masks { get; set; }
    public ClothingModel Torsos { get; set; }
    public ClothingModel Legs { get; set; }
    public ClothingModel Bags { get; set; }
    public ClothingModel Shoes { get; set; }
    public ClothingModel Accessories { get; set; }
    public ClothingModel Undershirts { get; set; }
    public ClothingModel Decals { get; set; }
    public ClothingModel Tops { get; set; }
    public ClothingModel Hats { get; set; }
    public ClothingModel Glasses { get; set; }
    public ClothingModel Ears { get; set; }
    public ClothingModel Watches { get; set; }
    public ClothingModel Bracelets { get; set; }

    public OutfitModel()
    {
    }

    public OutfitModel(int playerId, string name, ClothesModel clothes)
    {
        PlayerId = playerId;
        Name = name;
        Masks = clothes.Mask;
        Torsos = clothes.Body;
        Legs = clothes.Leg;
        Bags = clothes.Bag;
        Shoes = clothes.Shoe;
        Accessories = clothes.Accessories;
        Undershirts = clothes.Undershirt;
        Decals = clothes.Decals;
        Tops = clothes.Top;
        Hats = clothes.Hat;
        Glasses = clothes.Glasses;
        Ears = clothes.Ears;
        Watches = clothes.Watch;
        Bracelets = clothes.Bracelet;
    }
}