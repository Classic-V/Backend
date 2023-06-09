using Backend.Controllers.Event.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Interfaces;
using Backend.Utils.Models.Entities;

namespace Backend.Utils.Models
{
	public abstract class ModuleBase : IModuleBase
	{
		public string Name { get; }
		public bool Enabled { get; }

		protected ModuleBase(string name, bool enabled)
		{
			Name = name;
			Enabled = enabled;
		}
	}

	public abstract class Module<T> : ModuleBase where T : Module<T>
	{
		protected Module(string name, bool enabled = true) : base(name, enabled)
		{
		}
	}

	public abstract class CommandModule<T> : ModuleBase where T : CommandModule<T>
	{
		private AdminRank MinimumRank { get; }

		protected CommandModule(string name, AdminRank rank = AdminRank.PROJEKTLEITER, bool enabled = true) : base(name, enabled)
		{
			MinimumRank = rank;
		}

		protected bool CheckPermission(ClPlayer player)
		{
			return player.DbModel != null && player.DbModel.AdminRank >= MinimumRank;
		}
	}
}