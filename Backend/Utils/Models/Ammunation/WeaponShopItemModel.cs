namespace Backend.Utils.Models.Ammunation;

public class WeaponShopItemModel
{
    public int ItemId { get; set; }
    public string ItemName { get; set; }
    public int Price { get; set; }

    public WeaponShopItemModel(int itemId, string itemName, int price)
    {
        this.ItemId = itemId;
        this.ItemName = itemName;
        this.Price = price;
    }
}