using AltV.Net.Data;

namespace Backend.Utils.Models.Vehicle
{
	public class TuningModel
	{
		public RgbModel PrimaryColor { get; set; } = new(0, 0, 0, 255);
		public RgbModel SecondaryColor { get; set; } = new(0, 0, 0, 255);
		public byte PearlColor { get; set; }
		public byte Spoiler { get; set; }
		public byte FrontBumper { get; set; }
		public byte RearBumper { get; set; }
		public byte SideSkirt { get; set; }
		public byte Exhaust { get; set; }
		public byte Frame { get; set; }
		public byte Grille { get; set; }
		public byte Hood { get; set; }
		public byte Fender { get; set; }
		public byte RightFender { get; set; }
		public byte Roof { get; set; }
		public byte Engine { get; set; }
		public byte Brakes { get; set; }
		public byte Transmission { get; set; }
		public byte Horns { get; set; }
		public byte Suspension { get; set; }
		public byte Armor { get; set; }
		public byte Turbo { get; set; }
		public byte Xenon { get; set; }
		public byte Wheels { get; set; }
		public byte PlateHolders { get; set; }
		public byte TrimDesign { get; set; }
		public byte WindowTint { get; set; }
		public byte HeadlightColor { get; set; }
		public bool Neons { get; set; }
		public RgbModel NeonColor { get; set; } = new(0, 0, 0, 255);
		public byte Livery { get; set; }

		public TuningModel() { }
	}
}