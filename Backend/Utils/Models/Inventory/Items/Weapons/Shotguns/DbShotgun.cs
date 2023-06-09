namespace Backend.Utils.Models.Inventory.Items.Weapons.Shotguns;

public class DbShotgun : WeaponItemBase
{
    public override int Id => 247;
    public DbShotgun() : base("Db Shotgun", 0xEF951FBB, Enums.WeaponType.SHOTGUN, Enums.InjuryType.SHOT_HIGH)
    {
    }

}