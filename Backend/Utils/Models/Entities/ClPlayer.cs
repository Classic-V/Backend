using AltV.Net;
using AltV.Net.Async.Elements.Entities;
using AltV.Net.Data;
using Backend.Data;
using Backend.Modules.World;
using Backend.Utils.Enums;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Player;
using Backend.Utils.Models.Player.Client;
using Backend.Utils.Streamer;
using Newtonsoft.Json;
using Backend.Utils.Models.Bank;
using Backend.Utils.Models.Input;

namespace Backend.Utils.Models.Entities
{
	public class ClPlayer : AsyncPlayer
	{
		public static readonly List<ClPlayer> All = new();
		public static List<ClPlayer> LoggedIn
		{
			get => All.Where(x => x.DbModel != null).ToList();
		}

		private static readonly string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

		public AccountModel DbModel { get; set; } = null!;
		public DrugModel DrugState { get; set; } = new();
		public DateTime DeathTime { get; set; }
		public DateTime KomaTime { get; set; }
		public string EventKey { get; set; }
		public ClShape? CurrentShape { get; set; } = null;

		public bool IsFarming { get; set; } = false;
		public int CurrentFarmingSpot { get; set; } = 0;
		public int CurrentFarmingIndex { get; set; } = 0;

		public bool IsInInterior { get; set; }
		public Position OutsideInteriorPosition { get; set; }

		public bool IsInGangwar { get; set; }
		public int GangwarWeapon { get; set; } = -1;
		public bool GangwarSpecialWeapon { get; set; } = false;
		public List<LoadoutModel> GangwarCachedWeapons { get; set; }

		public bool Aduty { get; set; } = false;
		public bool Vanish { get; set; } = false;

		public bool InHostpital { get; set; } = false;

		private Task? InteractionTask { get; set; }
		private CancellationTokenSource InteractionCancelToken { get; set; } = new CancellationTokenSource();
		private string ClientInteraction { get; set; } = string.Empty;
		private string ClientCachedInteraction { get; set; } = string.Empty;

		public bool LaboratoryStatus { get; set; } = false;
		public bool HasBeenSearched { get; set; } = false;

		public bool Interaction => InteractionTask != null;

        public bool IsInPrison { get; set; } = false;

		public bool IsTraining { get; set; } = false;
		public DateTime TrainingStart { get; set; } = DateTime.Now;
        public Action? OnInteractionCancel { get; set; }
		public DateTime LastLifeinvaderPost { get; set; } = DateTime.Now.AddMinutes(-15);
		public bool IsInFFA { get; set; } = false;
		public int FFAId { get; set; }

        public ClPlayer(ICore core, IntPtr nativePointer, ushort id) : base(core, nativePointer, id)
		{
			All.Add(this);
			EventKey = new string(Enumerable.Repeat(Chars, 20).Select(s => s[new Random().Next(s.Length)]).ToArray());
			Emit("Client:Script:SetEventKey", EventKey);
		}

		public Task SetMoney(int money)
		{
			DbModel.Money = money;
			EmitBrowser("Hud:SetMoney", money);
			return Task.CompletedTask;
		}

		public async Task AddMoney(int money)
		{
			await SetMoney(DbModel.Money + money);
		}

		public async Task RemoveMoney(int money)
		{
			await SetMoney(DbModel.Money - money);
		}

		public Task Copy(string value)
		{
			Emit("Client:BrowserModule:Copy", value);
			return Task.CompletedTask;
		}

		public Task SetModel(uint model)
		{
			Model = model;
			return Task.CompletedTask;
		}

		public Task SetHealth(ushort health, ushort armor, bool apply = true)
		{
			Emit("Client:AnticheatModule:SetHealth", health + armor);
			if (!apply) return Task.CompletedTask;
			MaxHealth = (ushort)(health >= 200 ? health : 200);
			Health = health;
			MaxArmor = (ushort)(armor >= 100 ? armor : 100);
			Armor = armor;
			return Task.CompletedTask;
		}

		public Task SetGodmode(bool state)
		{
			Invincible = state;
			Emit("Client:AnticheatModule:SetGodmode", state);
			SetStreamSyncedMetaData("GODMODE", state);
			return Task.CompletedTask;
		}

		public Task SetInvisible(bool state)
		{
			Visible = state;
			return Task.CompletedTask;
		}

		public Task SetPosition(Position position)
		{
			Position = position;
			Emit("Client:AnticheatModule:SetPosition", position);
			return Task.CompletedTask;
		}

		public Task SetFood(int starvation, int hydration, float strength, bool apply = true)
		{
			EmitBrowser("Hud:SetFood", starvation, hydration, strength);
			Emit("Client:PlayerModule:SetRunSpeedMultiplier", Math.Clamp(strength, 80, 110) / 100);

			if (apply)
			{
				DbModel.Starvation = starvation;
				DbModel.Hydration = hydration;
				DbModel.Strength = strength;
			}

			return Task.CompletedTask;
		}

		public Task SetTeam(int team)
		{
			Emit("Client:PlayerModule:SetTeam", team);

			return Task.CompletedTask;
		}

		public Task Notify(string title, string msg, NotificationType type, int duration = 3500)
		{
			Emit("Client:Hud:PushNotification", JsonConvert.SerializeObject(new ClientNotificationModel(title, msg, type, duration)));
			return Task.CompletedTask;
		}

		public Task ShowGlobalNotify(string title, string msg, int duration)
		{
			EmitBrowser("Hud:ShowGlobalNotification", JsonConvert.SerializeObject(new ClientNotificationModel(title, msg, 0, duration)));
			return Task.CompletedTask;
		}

		public async Task ShowInput(string title, string message, InputType type, string callback, params object[] data)
		{
			await ShowComponent("Input", true,
				JsonConvert.SerializeObject(new InputDataModel(title, message, type, callback, data)));
		}

		public Task ApplyWeapons()
		{
			RemoveAllWeapons();
			var weapons = new List<ClientWeaponModel>();
			DbModel.Loadout.ForEach(x =>
			{
				weapons.Add(new ClientWeaponModel(x.Hash, x.Ammo, x.Attatchments));
				GiveWeapon(x.Hash, x.Ammo, false);
				x.Attatchments.ForEach(e => AddWeaponComponent(x.Hash, e));
				SetWeaponTintIndex(x.Hash, x.TintIndex);
			});
			Emit("Client:PlayerModule:SetWeapons", JsonConvert.SerializeObject(weapons));
			return Task.CompletedTask;
		}

		public async Task SetWeapons(List<LoadoutModel> weapons)
		{
			DbModel.Loadout = weapons;
			await ApplyWeapons();
		}

		public Task AddWeapon(uint weapon, int ammo, List<uint> components, byte tintIndex)
		{
			DbModel.Loadout.Add(new LoadoutModel(weapon, ammo, components, tintIndex));
			Emit("Client:PlayerModule:AddWeapon", JsonConvert.SerializeObject(new ClientWeaponModel(weapon, ammo, components)));
			GiveWeapon(weapon, ammo, true);
			components.ForEach(x => AddWeaponComponent(weapon, x));
			SetWeaponTintIndex(weapon, tintIndex);
			return Task.CompletedTask;
		}

		public Task RemoveWeapon2(uint weapon)
		{
			var targetWeapon = DbModel.Loadout.FirstOrDefault(x => x.Hash == weapon);
			if (targetWeapon == null) return Task.CompletedTask;

			DbModel.Loadout.Remove(targetWeapon);
			RemoveWeapon(weapon);
			Emit("Client:PlayerModule:RemoveWeapon", weapon);
			return Task.CompletedTask;
		}

		public Task ApplyCustomization()
		{
			if (DbModel == null) return Task.CompletedTask;

			Model = DbModel.Customization.Gender == 1 ? 0x705E61F2 : 0x9C9EFFD8;
			Emit("Client:Creator:SetCustomization", JsonConvert.SerializeObject(DbModel.Customization), true);
			return Task.CompletedTask;
		}

		public Task ShowComponent(string name, bool state, string data = "")
		{
			Emit("Client:BrowserModule:ShowComponent", name, state, data);
			return Task.CompletedTask;
		}

		public Task EmitBrowser(string eventName, params object[] args)
		{
			Emit("Client:BrowserModule:CallEvent", eventName, args);
			return Task.CompletedTask;
		}

		public Task StartInteraction(Action action, int duration, Action onCancel = null)
		{
			Emit("Client:Hud:SetProgressbarState", true, duration);
			ClientCachedInteraction = ClientInteraction;
			SetInteraction(Interactions.KEY_E, Interactions.E_INTERACTION);

			OnInteractionCancel = onCancel;

            InteractionTask = Task.Run(async () =>
			{
				await Task.Delay(duration, InteractionCancelToken.Token);

				action();

				await StopInteraction();
			}, InteractionCancelToken.Token);
			return Task.CompletedTask;
		}

		public Task StopInteraction()
		{
			if (Interaction)
			{
                if (OnInteractionCancel != null)
                {
					OnInteractionCancel.Invoke();
                }

				InteractionCancelToken.Cancel();
				InteractionCancelToken = new CancellationTokenSource();
				InteractionTask = null;
				OnInteractionCancel = null;
				StopAnimation();
				SetInteraction(Interactions.KEY_E, ClientCachedInteraction);
				Emit("Client:Hud:SetProgressbarState", false);
			}

			return Task.CompletedTask;
		}

		public void SetInteraction(string key, string interaction)
		{
			ClientInteraction = interaction;
			Emit("Client:KeyHandler:SetInteraction", key, interaction);
		}

		public Task Update()
		{
			DbModel.LastPos = new PositionModel(IsInInterior ? OutsideInteriorPosition : Position, Rotation.Yaw);
			DbModel.Health = Health;
			DbModel.Armor = Armor;

			return Task.CompletedTask;
		}

		public async Task Load()
		{
			if (DbModel == null!) return;

			Emit("Client:PlayerModule:SetTeam", DbModel.Team);
			Emit("Client:PlayerModule:SetAdmin", (int)DbModel.AdminRank);
			// Todo: add cuff & rope to db
			SetStreamSyncedMetaData("ALIVE", DbModel.Alive);
			SetStreamSyncedMetaData("ROPED", DbModel.Roped);
			SetStreamSyncedMetaData("CUFFED", DbModel.Cuffed);
			SetStreamSyncedMetaData("STABILIZED", DbModel.Stabilized);

			Emit("Client:PlayerModule:SetCuffed", DbModel.Roped || DbModel.Cuffed);

			await ShowComponent("Hud", true);
			await SetFood(DbModel.Starvation, DbModel.Hydration, DbModel.Strength, false);
			await EmitBrowser("Hud:ShowInfo", true, JsonConvert.SerializeObject(new ClientHudData(DbModel.Money, DbModel.Starvation, DbModel.Hydration, DbModel.Strength)));
			await ApplyCustomization();
			Spawn(Position);
			await SetPosition(DbModel.LastPos.Position);
			Rotation = new Rotation(0, 0, (float)DbModel.LastPos.H);
			await SetHealth(DbModel.Health, DbModel.Armor);
			await ApplyWeapons();
			await ApplyClothes();
			await SetGodmode(false);
			await SetDimension(0);

			DbModel.LastLogin = DateTime.Now;

			if (!DbModel.Alive || DbModel.IsKoma)
			{
				DeathTime = DateTime.Now;
				KomaTime = DateTime.Now;
				Emit("Client:PlayerModule:SetAlive", false);
				await SetGodmode(true);
			}

			Emit("Client:PlayerModule:SetRunSpeedMultiplier", DbModel.Strength / 100);

			// Initialize streamer
			Emit("Client:ObjectStreamer:SetObjects", JsonConvert.SerializeObject(ObjectStreamer.Objects));
			Emit("Client:PedStreamer:SetPeds", JsonConvert.SerializeObject(PedStreamer.Peds));
			Emit("Client:MarkerStreamer:SetMarkers", JsonConvert.SerializeObject(MarkerStreamer.Markers));
			Emit("Client:PlayerModule:SetWeather", WorldModule.Weather);
		}

		public Task SetStabilized(bool state)
		{
			DbModel.Stabilized = state;
			SetStreamSyncedMetaData("STABILIZED", state);
			return Task.CompletedTask;
		}

		public Task SetCuffed(bool state)
		{
			DbModel.Cuffed = state;
			SetStreamSyncedMetaData("CUFFED", state);
			Emit("Client:PlayerModule:SetCuffed", state);
			return Task.CompletedTask;
		}

		public Task SetRoped(bool state)
		{
			DbModel.Roped = state;
			SetStreamSyncedMetaData("ROPED", state);
			Emit("Client:PlayerModule:SetCuffed", state);
			return Task.CompletedTask;
        }

        public Task ApplyClothes(ClothesModel clothes)
        {
            var model = clothes;

            SetClothing(1, model.Mask.Drawable, model.Mask.Texture, model.Mask.Dlc);
            SetClothing(10, model.Decals.Drawable, model.Decals.Texture, model.Decals.Dlc);
            SetClothing(11, model.Top.Drawable, model.Top.Texture, model.Top.Dlc);
            SetClothing(8, model.Undershirt.Drawable, model.Undershirt.Texture, model.Undershirt.Dlc);
            SetClothing(3, model.Body.Drawable, model.Body.Texture, model.Body.Dlc);
            SetClothing(4, model.Leg.Drawable, model.Leg.Texture, model.Leg.Dlc);
            SetClothing(6, model.Shoe.Drawable, model.Shoe.Texture, model.Shoe.Dlc);
            SetClothing(9, model.Armor.Drawable, model.Armor.Texture, model.Armor.Dlc);
            SetClothing(5, model.Bag.Drawable, model.Bag.Texture, model.Bag.Dlc);
            SetClothing(7, model.Accessories.Drawable, model.Accessories.Texture, model.Accessories.Dlc);

            SetProp(0, model.Hat.Drawable, model.Hat.Texture, model.Hat.Dlc);
            SetProp(1, model.Glasses.Drawable, model.Glasses.Texture, model.Glasses.Dlc);
            SetProp(2, model.Ears.Drawable, model.Ears.Texture, model.Ears.Dlc);
            SetProp(6, model.Watch.Drawable, model.Watch.Texture, model.Watch.Dlc);
            SetProp(7, model.Bracelet.Drawable, model.Bracelet.Texture, model.Bracelet.Dlc);
            return Task.CompletedTask;
        }

        public Task ApplyClothes()
        {
            var model = DbModel.Clothes;

            SetClothing(1, model.Mask.Drawable, model.Mask.Texture, model.Mask.Dlc);
            SetClothing(10, model.Decals.Drawable, model.Decals.Texture, model.Decals.Dlc);
            SetClothing(11, model.Top.Drawable, model.Top.Texture, model.Top.Dlc);
            SetClothing(8, model.Undershirt.Drawable, model.Undershirt.Texture, model.Undershirt.Dlc);
            SetClothing(3, model.Body.Drawable, model.Body.Texture, model.Body.Dlc);
            SetClothing(4, model.Leg.Drawable, model.Leg.Texture, model.Leg.Dlc);
            SetClothing(6, model.Shoe.Drawable, model.Shoe.Texture, model.Shoe.Dlc);
            SetClothing(9, model.Armor.Drawable, model.Armor.Texture, model.Armor.Dlc);
            SetClothing(5, model.Bag.Drawable, model.Bag.Texture, model.Bag.Dlc);
            SetClothing(7, model.Accessories.Drawable, model.Accessories.Texture, model.Accessories.Dlc);

            SetProp(0, model.Hat.Drawable, model.Hat.Texture, model.Hat.Dlc);
            SetProp(1, model.Glasses.Drawable, model.Glasses.Texture, model.Glasses.Dlc);
            SetProp(2, model.Ears.Drawable, model.Ears.Texture, model.Ears.Dlc);
            SetProp(6, model.Watch.Drawable, model.Watch.Texture, model.Watch.Dlc);
            SetProp(7, model.Bracelet.Drawable, model.Bracelet.Texture, model.Bracelet.Dlc);
            return Task.CompletedTask;
        }

        public Task SetClothing(int slot, int drawable, int texture, uint dlc)
		{
			if(dlc == 0) SetClothes((byte)slot, (ushort)drawable, (byte)texture, 0);
			else SetDlcClothes((byte)slot, (ushort)drawable, (byte)texture, 0, dlc);
			return Task.CompletedTask;
		}

		public Task SetProp(int slot, int drawable, int texture, uint dlc)
		{
			if(drawable == -1)
			{
				ClearProps((byte)slot);
				return Task.CompletedTask;
			}

			if (dlc == 0) SetProps((byte)slot, (ushort)drawable, (byte)texture);
			else SetDlcProps((byte)slot, (ushort)drawable, (byte)texture, dlc);
			return Task.CompletedTask;
        }

        public void PlayAnimation(AnimationType type)
        {
            Emit("Client:Animation:Play", (int)type);
        }

        public void PlayAnimation(string animationDictionary, string animationName, int animationFlag)
        {
            Emit("Client:Animation:PlayTask", animationDictionary, animationName, animationFlag);
        }

        public void StopAnimation()
		{
			Emit("Client:Animation:ClearTasks");
		}

		public Task ShowNativeMenu(bool state, ClientNativeMenu menu)
		{
			var data = state ? JsonConvert.SerializeObject(menu) : "";
			Emit("Client:Hud:ShowNativeMenu", state, data);
			return Task.CompletedTask;
		}

		public Task Freeze(bool state)
		{
			Emit("Client:PlayerModule:Freeze", state);
			return Task.CompletedTask;
		}

		public Task SetPlayerFarming(bool state)
		{
			Emit("Client:PlayerModule:SetFarming", state);
			return Task.CompletedTask;
		}

		public Task SetDimension(int dimension)
		{
			Emit("Client:PlayerModule:SetDimension", dimension);
			Dimension = dimension;
			return Task.CompletedTask;
		}

        public Task CreateTransactionHistory(string bankName, TransactionType type, int amount)
        {
			DbModel.TransactionHistory.Add(new TransactionHistoryModel(bankName, type, amount, DateTime.Now.ToLocalTime()));

            return Task.CompletedTask;
        }

        public bool HasCrimes()
        {
            return DbModel.Crimes.Find(x => x.CrimeId >= 1) != null;
        }
    }
}