namespace Backend.Utils.Models.Player
{
	public class LoadoutModel
	{
		public uint Hash { get; set; }
		public int Ammo { get; set; }
		public List<uint> Attatchments { get; set; }
		public byte TintIndex { get; set; }

		public LoadoutModel(uint hash, int ammo, List<uint> attatchments, byte tintIndex)
		{
			Hash = hash;
			Ammo = ammo;
			Attatchments = attatchments;
			TintIndex = tintIndex;
		}
	}
}