namespace Backend.Utils.Models.Inventory.Items.Weapons.Shotguns;

public class SawnOffShotgun : WeaponItemBase
{
    public override int Id => 242;
    public SawnOffShotgun() : base("Sawn Off Shotgun", 0x7846A318, Enums.WeaponType.SHOTGUN, Enums.InjuryType.SHOT_HIGH)
    {
    }

}