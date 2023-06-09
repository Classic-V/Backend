namespace Backend.Utils.Models.ClothesShop;

public class ClothingShopItemModel
{
    public int Id { get; set; }
    public ClothingModel Clothing { get; set; }
    public int Price { get; set; }
    public string Name { get; set; }
    public bool Gender { get; set; }
    public bool Prop { get; set; }

    public ClothingShopItemModel(int id, ClothingModel clothing, int price, string name, bool prop)
    {
        Id = id;
        Clothing = clothing;
        Price = price;
        Name = name;
        Prop = prop;
    }
}