using Backend.Utils.Configurations;
using Backend.Utils.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MySqlConnector;
using System.Diagnostics;

namespace Backend.Utils
{
	public class DBContext : DbContext
	{
		public DbSet<AccountModel> Accounts { get; set; }
		public DbSet<DropModel> Drops { get; set; }
		public DbSet<GarageModel> Garages { get; set; }
		public DbSet<VehicleModel> Vehicles { get; set; }
		public DbSet<TeamModel> Teams { get; set; }
		public DbSet<StorageModel> Storages { get; set; }
		public DbSet<JumpPointModel> JumpPoints { get; set; }
		public DbSet<FarmingModel> FarmingSpots { get; set; }
		public DbSet<ProcessorModel> Processors { get; set; }
		public DbSet<GangwarModel> Gangwars { get; set; }
		public DbSet<ShopModel> Shops { get; set; }
		public DbSet<ClothesShopModel> ClothesShop { get; set; }
		public DbSet<DealerModel> Dealer { get; set; }
		public DbSet<BankModel> Bank { get; set; }
		public DbSet<VehicleInfoModel> VehicleInfos { get; set; }
		public DbSet<GasStationModel> GasStations { get; set; }
		public DbSet<WorkstationModel> Workstations { get; set; }
		public DbSet<VehicleShopModel> VehicleShops { get; set; }
		public DbSet<BlipModel> Blips { get; set; }
		public DbSet<CrimeModel> Crimes { get; set; }
		public DbSet<WardrobeModel> Wardrobes { get; set; }
		public DbSet<OutfitModel> Outfits { get; set; }
		public DbSet<HouseModel> House { get; set; }
        public DbSet<JailModel> Jail { get; set; }
        public DbSet<AnimationModel> Animations { get; set; }
		public DbSet<TrainingStationModel> TrainingStation { get; set; }
		public DbSet<ClothesShopItemModel> ClothesShopItem { get; set; }
		public DbSet<AmmunationModel> Ammunation { get; set; }
        public DbSet<PostJobModel> PostJob { get; set; }
        public DbSet<DoorModel> Door { get; set; }
        public DbSet<MoneyTransportJobModel> MoneyTransportJob { get; set; }
		public DbSet<PizzaDeliveryJobModel> PizzaDeliveryJobs { get; set; }
		public DbSet<FederalLicenseModel> FederalLicenses { get; set; }
        public DbSet<BarberModel> Barbers { get; set; }
        public DbSet<BarberItemModel> BarberItems { get; set; }
        public DbSet<FFAModel> FFA { get; set; }
		public DbSet<TattooShopModel> TattooShops { get; set; }

        public DBContext()
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				var connectionString = new MySqlConnectionStringBuilder
				{
					Server = "127.0.0.1",
					Port = 3306,
					UserID = "roleplay",
					Password = "83P3jYZL]8i3c3pf",
					Database = "roleplay",
				};

				optionsBuilder.UseMySql(connectionString.ConnectionString, ServerVersion.AutoDetect(connectionString.ConnectionString), options => options.EnableRetryOnFailure(
						maxRetryCount: 5,
						maxRetryDelay: TimeSpan.FromSeconds(30),
						errorNumbersToAdd: null))
					.LogTo(error => Debug.Write(error), new[] { RelationalEventId.CommandExecuted })
					.EnableSensitiveDataLogging();
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(DBContext).Assembly); 
		}
	}
}