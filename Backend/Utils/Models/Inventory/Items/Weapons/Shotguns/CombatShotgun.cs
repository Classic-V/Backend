namespace Backend.Utils.Models.Inventory.Items.Weapons.Shotguns;

public class CombatShotgun : WeaponItemBase
{
    public override int Id => 240;
    public CombatShotgun() : base("Combat Shotgun", 0x5A96BA4, Enums.WeaponType.SHOTGUN, Enums.InjuryType.SHOT_HIGH)
    {
    }

}