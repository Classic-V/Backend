using AltV.Net;
using AltV.Net.Async.Elements.Entities;
using AltV.Net.Data;
using Backend.Modules.Inventory;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Inventory;

namespace Backend.Utils.Models.Entities
{
	public class ClVehicle : AsyncVehicle
	{
		public static readonly List<ClVehicle> All = new();

		public static ClVehicle getVehicle(int id)
		{
			return All.FirstOrDefault(x => x.DbModel?.Id == id);
		}

		public VehicleModel? DbModel { get; set; } = null;
		public bool Locked { get; set; }
		public bool TrunkLocked { get; set; }
		public bool Engine { get; set; }
		DateTime LastUse { get; set; }

		public int MaxFuel { get; }

		public bool SirenSoundActive { get; private set; } = true;

		public int GangwarId { get; set; } = -1;

		public ClVehicle(ICore core, uint model, Position position, Rotation rotation, VehicleModel? dbModel, int maxFuel) : base(core, model, position, rotation)
		{
			if (!Exists) return;
			DbModel = dbModel;
			SetLockState(true);
			TrunkLocked = true;
			SetEngineState(false);
			LastUse = DateTime.Now;
			MaxFuel = maxFuel;
			SetStreamSyncedMetaData("MAX_FUEL", maxFuel);
			SetFuel(dbModel == null ? maxFuel : dbModel.Fuel);

			if (dbModel != null)
			{
				NumberplateText = dbModel.Plate;
			}

			ModKit = 1;
			ApplyCustomization().GetAwaiter();
			All.Add(this);
		}

		public ClVehicle(ICore core, IntPtr entityPointer, ushort id) : base(core, entityPointer, id)
		{
		}

		~ClVehicle()
		{
			All.Remove(this);
		}

		public Task SetEngineState(bool state)
		{
			Engine = state;
			EngineOn = state;
			SetStreamSyncedMetaData("ENGINE", state);
			return Task.CompletedTask;
		}

		public Task SetLockState(bool state)
		{
			Locked = state;
			LockState = state ? AltV.Net.Enums.VehicleLockState.Locked : AltV.Net.Enums.VehicleLockState.Unlocked;
			SetStreamSyncedMetaData("LOCKED", state);
			TrunkLocked = true;
			SetDoorState(5, 0);
			return Task.CompletedTask;
		}

		public Task SetFuel(float val)
		{
			var fuel = Math.Clamp(val, 0, MaxFuel);
			if(DbModel != null) DbModel.Fuel = fuel;
			SetStreamSyncedMetaData("FUEL", fuel);
			if (fuel == 0) SetEngineState(false);
			return Task.CompletedTask;
		}

		public Task Update()
		{
			if (DbModel == null) return Task.CompletedTask;

			DbModel.Position = new PositionModel(Position.X, Position.Y, Position.Z, Rotation.Yaw);

			return Task.CompletedTask;
		}

		public Task ApplyCustomization()
		{
			if (DbModel == null) return Task.CompletedTask;

			var tune = DbModel.Tuning;

			PrimaryColorRgb = tune.PrimaryColor.Rgba;
			SecondaryColorRgb = tune.SecondaryColor.Rgba;
			PearlColor = tune.PearlColor;
			NeonColor = tune.NeonColor.Rgba;

			WindowTint = tune.WindowTint;
			HeadlightColor = tune.HeadlightColor;

			SetMod(0, tune.Spoiler);
			SetMod(1, tune.FrontBumper);
			SetMod(2, tune.RearBumper);
			SetMod(3, tune.SideSkirt);
			SetMod(4, tune.Exhaust);
			SetMod(5, tune.Frame);
			SetMod(6, tune.Grille);
			SetMod(7, tune.Hood);
			SetMod(8, tune.Fender);
			SetMod(9, tune.RightFender);
			SetMod(10, tune.Roof);
			SetMod(11, tune.Engine);
			SetMod(12, tune.Brakes);
			SetMod(13, tune.Transmission);
			SetMod(14, tune.Horns);
			SetMod(15, tune.Suspension);
			SetMod(16, tune.Armor);
			SetMod(18, tune.Turbo);
			SetMod(22, tune.Xenon);
			SetMod(23, tune.Wheels); // SetWheels()?
			SetMod(25, tune.PlateHolders);
			SetMod(27, tune.TrimDesign);

			SetNeonActive(tune.Neons, tune.Neons, tune.Neons, tune.Neons);
			return Task.CompletedTask;
		}

		public Task SetSirenSoundActive(bool state)
		{
			SirenSoundActive = state;
			SetStreamSyncedMetaData("SIREN_SOUND", state);
			return Task.CompletedTask;
		}
	}
}