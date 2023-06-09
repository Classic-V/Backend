namespace Backend.Utils.Models.Inventory.Items.Weapons.Shotguns;

public class Musket : WeaponItemBase
{
    public override int Id => 245;
    public Musket() : base("Muskete", 0xA89CB99E, Enums.WeaponType.SHOTGUN, Enums.InjuryType.SHOT_HIGH)
    {
    }

}