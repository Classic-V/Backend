using Backend.Utils.Enums;

namespace Backend.Utils.Models.Database;

public class ClothesShopItemModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Component { get; set; }
    public int Drawable { get; set; }
    public int Texture { get; set; }
    public string Dlc { get; set; }
    public int Price { get; set; }
    public bool IsProp { get; set; }
    public bool Gender { get; set; }
    public ClothesShopItemType  Type { get; set; }

    public ClothesShopItemModel() {}

    public ClothesShopItemModel(string name, int component, int drawable, int texture, string dlc, int price, bool isProp, bool gender, ClothesShopItemType type)
    {
        Name = name;
        Component = component;
        Drawable = drawable;
        Texture = texture;
        Dlc = dlc;
        Price = price;
        IsProp = isProp;
        Gender = gender;
        Type = type;
    }
}