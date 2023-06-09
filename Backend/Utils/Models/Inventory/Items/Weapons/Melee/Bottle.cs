namespace Backend.Utils.Models.Inventory.Items.Weapons.Melee;

public class Bottle : WeaponItemBase
{
    public Bottle() : base("Flasche", 0xF9E6AA4B, Enums.WeaponType.MEELE, Enums.InjuryType.SLICE)
    {
    }

    public override int Id => 182;
}