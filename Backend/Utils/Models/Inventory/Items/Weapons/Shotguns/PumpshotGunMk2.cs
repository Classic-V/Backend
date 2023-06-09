namespace Backend.Utils.Models.Inventory.Items.Weapons.Shotguns;

public class PumpshotGunMk2 : WeaponItemBase
{
    public override int Id => 241;
    public PumpshotGunMk2() : base("Pumpshot Gun MKII", 0x555AF99A, Enums.WeaponType.SHOTGUN, Enums.InjuryType.SHOT_HIGH)
    {
    }

}