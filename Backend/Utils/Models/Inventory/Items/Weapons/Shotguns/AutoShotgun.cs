namespace Backend.Utils.Models.Inventory.Items.Weapons.Shotguns;

public class Autoshotgun : WeaponItemBase
{
    public override int Id => 248;
    public Autoshotgun() : base("Autoshotgun", 0x12E82D3D, Enums.WeaponType.SHOTGUN, Enums.InjuryType.SHOT_HIGH)
    {
    }

}