using Backend.Utils.Models.Shops;

namespace Backend.Utils.Models.Ammunation;

public class WeaponShopDataModel
{

    public int Id { get; set; }
    public string Name { get; set; }
    public List<WeaponShopItemModel> Items { get; set; }

    public WeaponShopDataModel() { }

    public WeaponShopDataModel(int id, string name, List<WeaponShopItemModel> items)
    {
        Id = id;
        Name = name;
        Items = items;
    }
}