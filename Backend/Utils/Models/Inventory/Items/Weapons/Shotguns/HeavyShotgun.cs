namespace Backend.Utils.Models.Inventory.Items.Weapons.Shotguns;

public class HeavyShotgun : WeaponItemBase
{
    public override int Id => 246;
    public HeavyShotgun() : base("Heavyshotgun", 0x3AABBBAA, Enums.WeaponType.SHOTGUN, Enums.InjuryType.SHOT_HIGH)
    {
    }

}