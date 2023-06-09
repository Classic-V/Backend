using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Player.Client
{
	public class ClientWeaponModel
	{
		public uint hash { get; set; }
		public int ammo { get; set; }
		public List<uint> components { get; set; } = new();

		public ClientWeaponModel()
		{
		}

		public ClientWeaponModel(uint hash, int ammo, List<uint> components)
		{
			this.hash = hash;
			this.ammo = ammo;
			this.components = components;
		}
	}
}