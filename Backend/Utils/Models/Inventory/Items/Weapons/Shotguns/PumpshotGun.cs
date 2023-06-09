namespace Backend.Utils.Models.Inventory.Items.Weapons.Shotguns;

public class PumpshotGun : WeaponItemBase
{
    public override int Id => 240;
    public PumpshotGun() : base("Pumpshot Gun", 0x1D073A89, Enums.WeaponType.SHOTGUN, Enums.InjuryType.SHOT_HIGH)
    {
    }

}