namespace Backend.Utils.Models.Inventory.Items.Weapons.Shotguns;

public class AssaultShotGun : WeaponItemBase
{
    public override int Id => 243;
    public AssaultShotGun() : base("Assault Shotgun", 0xE284C527, Enums.WeaponType.SHOTGUN, Enums.InjuryType.SHOT_HIGH)
    {
    }

}