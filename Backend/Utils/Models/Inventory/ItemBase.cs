using Backend.Modules.Inventory;
using Backend.Utils.Enums;
using Backend.Utils.Models.Entities;

namespace Backend.Utils.Models.Inventory
{
	public abstract class ItemBase
	{
		public abstract int Id { get; }

		public string Name { get; }
		public float Weight { get; }
		public int MaxAmount { get; }
		public ItemType Type { get; }

		protected ItemBase(string name, float weight, int maxAmount, ItemType type = ItemType.ITEM)
		{
			Name = name;
			Weight = weight;
			MaxAmount = maxAmount;
			Type = type;

			InventoryModule.ItemModels.Add(this);
		}

		public abstract Task<bool> Use(ClPlayer player, int slot, int amount);
	}

	public abstract class WeaponItemBase : ItemBase
	{
		public uint Hash { get; set; }
		public WeaponType WeaponType { get; set; }
		public InjuryType WeaponInjury { get; set; }

		protected WeaponItemBase(string name, uint hash, WeaponType type, InjuryType weaponInjury) : base(name, 7f, 1, ItemType.WEAPON)
		{
			Hash = hash;
			WeaponType = type;
			WeaponInjury = weaponInjury;
		}

		public override Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			if (player.DbModel.Loadout.FirstOrDefault(x =>
			{
				WeaponItemBase model = (WeaponItemBase)InventoryModule.ItemModels.FirstOrDefault(e => e.Type == ItemType.WEAPON && ((WeaponItemBase)e).Hash == x.Hash);
				return model != null && model.WeaponType == WeaponType;
			}) != null) return Task.FromResult(false);

			player.AddWeapon(Hash, 0, new List<uint>(), 0);
			player.DbModel.Inventory.RemoveItem(slot, 1);
			return Task.FromResult(true);
		}
	}

	public abstract class AmmoItemBase : ItemBase
	{
		public WeaponType WeaponType { get; set; }

		protected AmmoItemBase(string name, float weight, int slotSize, WeaponType type) : base(name, weight, slotSize, ItemType.AMMO)
		{
			WeaponType = type;
		}

		public override async Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			var weapon = player.CurrentWeapon;
			var model = (WeaponItemBase)InventoryModule.ItemModels.FirstOrDefault(x => x.Type == ItemType.WEAPON && ((WeaponItemBase)x).Hash == weapon);
			if (model == null || model.WeaponType != WeaponType)
			{
				await player.Notify("FEHLER", "Du hast bereits eine Waffe dieser Kategorie ausgerüstet!", NotificationType.ERROR);
				return false;
			}

			player.PlayAnimation(AnimationType.PACK_GUN);
			await player.StartInteraction(() =>
			{
				var gun = player.DbModel.Loadout.FirstOrDefault(x => x.Hash == weapon);
				gun.Ammo = amount * 30;
				player.Emit("Client:PlayerModule:AddAmmo", amount * 30);
				player.DbModel.Inventory.RemoveItem(slot, amount);
			}, amount * 300);
			return true;
		}
	}
}