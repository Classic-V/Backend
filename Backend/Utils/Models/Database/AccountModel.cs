using System.ComponentModel.DataAnnotations.Schema;
using Backend.Utils.Enums;
using Backend.Utils.Models.Animation;
using Backend.Utils.Models.Bank;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Inventory;
using Backend.Utils.Models.Player;

namespace Backend.Utils.Models.Database
{
	public class AccountModel
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public ulong SocialClub { get; set; } = 0;
		public ulong HardwareId { get; set; } = 0;
		public ulong HardwareIdEx { get; set; } = 0;
		public long DiscordId { get; set; } = 0;
		public ushort Health { get; set; } = 200;
		public ushort Armor { get; set; } = 0;
		public PositionModel LastPos { get; set; } = new(-1042.4308f, -2746.0483f, 21.343628f, -0.54421294f);
		public List<LoadoutModel> Loadout { get; set; } = new();
		public bool Alive { get; set; } = true;
        public int Money { get; set; } = 0;
        public int BankMoney { get; set; } = 0;
        public AdminRank AdminRank { get; set; } = 0;
		public List<WarnModel> Warns { get; set; } = new();
		public InventoryModel Inventory { get; set; } = new(new List<ItemModel>(), 40f, 8, InventoryType.PLAYER);
		public CustomizationModel Customization { get; set; } = new();
		public ClothesModel Clothes { get; set; } = new();
		public int Team { get; set; } = -1;
		public int TeamRank { get; set; } = 0;
		public bool TeamAdmin { get; set; } = false;
		public bool TeamStoragePermission { get; set; } = false;
		public bool TeamBankPermission { get; set; } = false;
		public DateTime TeamJoinDate { get; set; }
		public int Business { get; set; } = -1;
		public int BusinessRank { get; set; } = 0;
		public bool BusinessAdmin { get; set; } = false;
		public int Level { get; set; } = 1;
		public int Xp { get; set; } = 0;
		public bool Phone { get; set; } = false;
		public bool Backpack { get; set; } = false;
		public int PaycheckTicks { get; set; } = 0;
		public bool IsKoma { get; set; } = false;
		public DateTime LastLogin { get; set; }
		public InventoryModel LaboratoryInput { get; set; } = new(52f, 8, InventoryType.LABORATORY_INPUT);
		public InventoryModel LaboratoryOutput { get; set; } = new(160f, 8, InventoryType.LABORATORY_OUTPUT);
		public int Starvation { get; set; } = 100;
		public int Hydration { get; set; } = 100;
		public bool Cuffed { get; set; } = false;
		public bool Roped { get; set; } = false;
		public bool Stabilized { get; set; } = false;
		public List<ClothesShopItemModel> WardrobeClothes { get; set; } = new();
		public int WorkstationId { get; set; } = 0;
		public List<PlayerWorkstationItemModel> WorkstationItems { get; set; } = new();
		public List<TransactionHistoryModel> TransactionHistory { get; set; } = new();
		public bool Duty { get; set; } = false;
        public bool SwatDuty { get; set; } = false;
        public bool SadDuty { get; set; } = false;
        public int SocialCredit { get; set; } = 0;
		public InjuryType Injury { get; set; }
		public bool Laptop { get; set; } = false;
		public ClothesModel? PlayerCachedClothes { get; set; } = null;
		public float Strength { get; set; } = 100;
        public List<PlayerCrimeModel> Crimes { get; set; } = new();
        public List<AnimationDataModel> FavoriteAnimations { get; set; } = new()
        {
            new AnimationDataModel(-2, "Nicht Belegt"),
            new AnimationDataModel(-2, "Nicht Belegt"),
            new AnimationDataModel(-2, "Nicht Belegt"),
            new AnimationDataModel(-2, "Nicht Belegt"),
            new AnimationDataModel(-2, "Nicht Belegt"),
            new AnimationDataModel(-2, "Nicht Belegt"),
            new AnimationDataModel(-2, "Nicht Belegt"),
            new AnimationDataModel(-2, "Nicht Belegt"),
            new AnimationDataModel(-2, "Nicht Belegt"),
            new AnimationDataModel(-2, "Nicht Belegt"),
        };
        public int Jailtime { get; set; } = 0;
		public int PhoneBackground { get; set; } = 0;

		// FEDERAL RECORDS
		public string FederalRecordTeam { get; set; } = string.Empty;
		public string FederalRecordDescription { get; set; } = string.Empty;
		public int FederalRecordPhone { get; set; } = 0;


		[NotMapped] public int GangwarId { get; set; } = -1;

		[NotMapped]
		public bool IsLoggedIn
		{
			get
			{
				return ClPlayer.LoggedIn.Find(x => x.DbModel.Id == Id) != null;
			}
		}

		public AccountModel() {}
		public AccountModel(string name, string password, ulong socialClub, ulong hardwareId, ulong hardwareIdEx, long discordId, ushort health, ushort armor, PositionModel lastPos, List<LoadoutModel> loadout, bool alive, int money, int bankMoney, AdminRank adminRank, InventoryModel inventory, CustomizationModel customization, ClothesModel clothes, int team, int teamRank, bool teamAdmin, bool teamStoragePermission, int level, int xp, bool phone, bool backpack, int paycheckTicks)
        {
            Name = name;
            Password = password;
            SocialClub = socialClub;
            HardwareId = hardwareId;
            HardwareIdEx = hardwareIdEx;
            DiscordId = discordId;
            Health = health;
            Armor = armor;
            LastPos = lastPos;
            Loadout = loadout;
            Alive = alive;
            Money = money;
			BankMoney = bankMoney;
            AdminRank = adminRank;
            Inventory = inventory;
            Customization = customization;
            Clothes = clothes;
            Team = team;
            TeamRank = teamRank;
            TeamAdmin = teamAdmin;
            TeamStoragePermission = teamStoragePermission;
            Level = level;
            Xp = xp;
            Phone = phone;
            Backpack = backpack;
            PaycheckTicks = paycheckTicks;
        }
    }
}