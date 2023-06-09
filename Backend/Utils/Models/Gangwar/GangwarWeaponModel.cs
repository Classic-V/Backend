namespace Backend.Utils.Models.Gangwar
{
	public class GangwarWeaponModel
	{
		public int Id { get; set; }
		public string Rifle { get; set; } = string.Empty;
		public string Pistol { get; set; } = string.Empty;
		public string Meele { get; set; } = string.Empty;

		public GangwarWeaponModel()
		{
		}

		public GangwarWeaponModel(int id, string rifle, string pistol, string meele)
		{
			Id = id;
			Rifle = rifle;
			Pistol = pistol;
			Meele = meele;
		}
	}
}