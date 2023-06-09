using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Inventory;
using Backend.Utils.Models.Player;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Backend.Modules.Inventory;
using Backend.Controllers.Gangwar.Interface;
using Backend.Utils.Enums;
using AltV.Net.Enums;
using Backend.Controllers.FFA.Interface;

namespace Backend.Modules.Death
{
	public class DeathModule : Module<DeathModule>, IEventIntervalMinute, IEventPlayerDeath
	{
		private readonly IGangwarController _gangwarController;
		private readonly IFFAController _ffaController;
		private readonly Position RespawnPosition = new(0, 0, 72);
		private readonly Dictionary<uint, InjuryType> SpecialInjuries = new()
		{
			{ Weapons.Fall, InjuryType.FALL_DAMAGE },
			{ Weapons.Exhaustion, InjuryType.FALL_DAMAGE },

			{ Weapons.Drowning, InjuryType.DROWN },
			{ Weapons.DrowningInVehicle, InjuryType.DROWN },

			{ Weapons.BarbedWire, InjuryType.SLICE },

			{ Weapons.Fire, InjuryType.FIRE },
			{ Weapons.ElectricFence, InjuryType.FIRE },

			{ Weapons.Grenade, InjuryType.EXPLOSION },
			{ Weapons.Explosion, InjuryType.EXPLOSION },
		};

		private readonly Dictionary<InjuryType, int> InjurySurviveTime = new()
		{
			{ InjuryType.FALL_DAMAGE, 15 },
			{ InjuryType.PUNCH, 15 },
			{ InjuryType.SLICE, 10 },
			{ InjuryType.DROWN, 5 },
			{ InjuryType.SHOT_LOW, 8 },
			{ InjuryType.VEHICLE, 10 },
			{ InjuryType.SHOT_HIGH, 5 },
			{ InjuryType.FIRE, 5 },
			{ InjuryType.EXPLOSION, 5 },
		};

		public DeathModule(IGangwarController gangwarController, IFFAController ffaController) : base("Death")
		{
			_gangwarController = gangwarController;
			_ffaController = ffaController;
		}

		public async Task OnEveryMinute()
		{
			foreach (var player in ClPlayer.LoggedIn.Where(x => (!x.DbModel.Alive || x.DbModel.IsKoma)))
			{
				if(player.DbModel.IsKoma)
				{
					if (player.KomaTime.AddMinutes(5) > DateTime.Now) continue;

					player.DrugState.Meth.Active = false;
					player.DrugState.Heroin.Active = false;
					player.DrugState.MDMA.Active = false;
					player.DrugState.Ecstasy.Active = false;
					player.DbModel.Alive = true;
					player.DbModel.Injury = 0;
					player.DbModel.IsKoma = false;
					player.DbModel.Backpack = false;
					player.DbModel.Phone = false;
					player.DbModel.Laptop = false;
					player.DbModel.Inventory.Items = new List<ItemModel>();
					player.DbModel.Inventory.Slots = 8;
					player.DbModel.Inventory.MaxWeight = 40;
					player.DbModel.Loadout = new List<LoadoutModel>();
					await player.ApplyWeapons();
					await player.SetPosition(RespawnPosition);
					player.Emit("Client:PlayerModule:SetAlive", true);
					player.StopAnimation();
					await player.SetHealth(200, 0);
					await player.SetGodmode(false);
					player.SetStreamSyncedMetaData("ALIVE", true);
					continue;
				}

				if (player.IsInGangwar)
				{
					if (player.DeathTime.AddMinutes(4) > DateTime.Now) continue;

					var gw = await _gangwarController.GetGangwarModel(player.Dimension);
					if (gw == null) continue;

					player.DbModel.Alive = true;
					player.DbModel.Injury = 0;
					await player.SetPosition(player.DbModel.Team == gw.AttackerId ? gw.AttackerSpawnPosition.Position : gw.DefenderSpawnPosition.Position);
					player.Emit("Client:PlayerModule:SetAlive", true);
					player.StopAnimation();
					await player.SetHealth(200, 100);
					await player.SetGodmode(false);

					player.DbModel.Inventory.AddItem(InventoryModule.GetItemBase(1500), 8);
					player.DbModel.Inventory.AddItem(InventoryModule.GetItemBase(1501), 8);
					continue;
				}

				var death = (int)Math.Round((decimal)InjurySurviveTime[player.DbModel.Injury] * (player.DbModel.Stabilized ? 2 : 1) + (player.DrugState.Meth.Active ? 5 : 0));

				if (player.DeathTime.AddMinutes(death) > DateTime.Now) continue;

				player.DbModel.IsKoma = true;
				player.KomaTime = DateTime.Now;
				await player.ShowComponent("Death", true, "1");
				await player.SetStabilized(false);
				await player.Notify("INFORMATION", "Du bist deinen Wunden erlegen und liegst nun im Koma.", NotificationType.INFO);
			}
		}

		public async Task OnPlayerDeath(ClPlayer player, IEntity? killer, uint weapon)
		{
			if (player.DbModel == null) return;

			if (player.Interaction) await player.StopInteraction();

			if (player.IsInGangwar)
			{
				await _gangwarController.PlayerDeath(player, killer);
			}

			if (player.IsInFFA)
			{
				await _ffaController.PlayerDeath(player, killer);
				return;
			}

			player.DbModel.Injury = GetInjury(weapon);
			player.DeathTime = DateTime.Now;
			player.DbModel.Alive = false;
			await Task.Delay(2000);
			player.Spawn(player.Position, 0);
			await player.SetHealth(200, 0, false);
			await player.SetGodmode(true);
			player.SetStreamSyncedMetaData("ALIVE", false);
			await player.SetCuffed(false);
			await player.SetRoped(false);

			player.Emit("Client:PlayerModule:SetAlive", false);
			return;
		}

		private InjuryType GetInjury(uint weapon)
		{
			if (weapon == (uint)WeaponModel.Fist) return InjuryType.PUNCH;

			var item = (WeaponItemBase?)InventoryModule.ItemModels.FirstOrDefault(x => x.Type == ItemType.WEAPON && ((WeaponItemBase)x).Hash == weapon);
			if (item != null) return item.WeaponInjury;

			if (SpecialInjuries.Keys.Contains(weapon)) return SpecialInjuries[weapon];

			Console.WriteLine("Unknown Injury: " + weapon);

			return 0;
		}
	}
}