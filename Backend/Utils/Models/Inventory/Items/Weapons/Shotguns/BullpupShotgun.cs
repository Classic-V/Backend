namespace Backend.Utils.Models.Inventory.Items.Weapons.Shotguns;

public class BullpupShotgun : WeaponItemBase
{
    public override int Id => 244;
    public BullpupShotgun() : base("Bullpup Shotgun", 0x9D61E50F, Enums.WeaponType.SHOTGUN, Enums.InjuryType.SHOT_HIGH)
    {
    }

}