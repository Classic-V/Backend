using Backend.Utils.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Backend.Utils.Models;
using Backend.Utils.Models.Inventory.Database;
using Backend.Utils.Models.Player;
using Newtonsoft.Json;
using Backend.Modules.Inventory;
using Backend.Utils.Models.Animation;
using Backend.Utils.Models.Bank;
using Backend.Utils.Models.ClothesShop;

namespace Backend.Utils.Configurations
{
	public class AccountModelConfiguration : IEntityTypeConfiguration<AccountModel>
	{
		public void Configure(EntityTypeBuilder<AccountModel> builder)
		{
			builder.HasKey(x => x.Id);
			builder.ToTable("accounts");
			builder.HasIndex(x => x.Id).HasDatabaseName("id");
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(16)");
            builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)");
            builder.Property(x => x.Password).HasColumnName("password").HasColumnType("varchar(255)");
			builder.Property(x => x.SocialClub).HasColumnName("social_club").HasColumnType("bigint");
			builder.Property(x => x.HardwareId).HasColumnName("hardware_id").HasColumnType("bigint");
			builder.Property(x => x.HardwareIdEx).HasColumnName("hardware_id_ex").HasColumnType("bigint");
			builder.Property(x => x.DiscordId).HasColumnName("discord").HasColumnType("bigint");
			builder.Property(x => x.Health).HasColumnName("health").HasColumnType("int(16)");
			builder.Property(x => x.Armor).HasColumnName("armor").HasColumnType("int(16)");
            builder.Property(x => x.LastPos).HasColumnName("last_pos").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<PositionModel>(y)!).HasColumnType("longtext");
			builder.Property(x => x.Loadout).HasColumnName("loadout").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<List<LoadoutModel>>(y)!).HasColumnType("longtext");
            builder.Property(x => x.Alive).HasColumnName("alive").HasColumnType("tinyint(1)");
            builder.Property(x => x.Money).HasColumnName("money").HasColumnType("int(32)");
            builder.Property(x => x.BankMoney).HasColumnName("bank_money").HasColumnType("int(32)");
            builder.Property(x => x.AdminRank).HasColumnName("admin_rank").HasColumnType("smallint");
			builder.Property(x => x.Ban).HasColumnName("ban").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<BanModel>(y)!).HasColumnType("longtext");
			builder.Property(x => x.Warns).HasColumnName("warns").HasConversion(
				y => JsonConvert.SerializeObject(y),
				y => JsonConvert.DeserializeObject<List<WarnModel>>(y)!).HasColumnType("longtext");
			builder.Property(x => x.Inventory).HasColumnName("inventory").HasConversion(
                y => JsonConvert.SerializeObject(InventoryModule.Convert(y)),
                y => InventoryModule.Convert(JsonConvert.DeserializeObject<DatabaseInventoryModel>(y)!)).HasColumnType("longtext");
            builder.Property(x => x.Customization).HasColumnName("customization").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<CustomizationModel>(y)!).HasColumnType("longtext");
            builder.Property(x => x.Clothes).HasColumnName("clothes").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<ClothesModel>(y)!).HasColumnType("longtext");
            builder.Property(x => x.Team).HasColumnName("team").HasColumnType("int(16)");
            builder.Property(x => x.TeamRank).HasColumnName("team_rank").HasColumnType("int(16)");
            builder.Property(x => x.TeamAdmin).HasColumnName("team_admin").HasColumnType("tinyint(1)");
            builder.Property(x => x.TeamStoragePermission).HasColumnName("team_storage_permission").HasColumnType("tinyint(1)");
			builder.Property(x => x.TeamBankPermission).HasColumnName("team_bank_permission").HasColumnType("tinyint(1)");
			builder.Property(x => x.TeamJoinDate).HasColumnName("team_join_date").HasColumnType("datetime");
			builder.Property(x => x.Business).HasColumnName("business").HasColumnType("int(16)");
			builder.Property(x => x.BusinessRank).HasColumnName("business_rank").HasColumnType("int(16)");
			builder.Property(x => x.BusinessAdmin).HasColumnName("business_admin").HasColumnType("tinyint(1)");
			builder.Property(x => x.Level).HasColumnName("level").HasColumnType("int(32)");
            builder.Property(x => x.Xp).HasColumnName("exp").HasColumnType("int(32)");
            builder.Property(x => x.Phone).HasColumnName("phone").HasColumnType("tinyint(1)");
            builder.Property(x => x.Backpack).HasColumnName("backpack").HasColumnType("tinyint(1)");
            builder.Property(x => x.PaycheckTicks).HasColumnName("paycheck_ticks").HasColumnType("int(16)");
			builder.Property(x => x.IsKoma).HasColumnName("koma").HasColumnType("tinyint(1)");
			builder.Property(x => x.LastLogin).HasColumnName("last_login").HasColumnType("datetime");
			builder.Property(x => x.LaboratoryInput).HasColumnName("lab_input").HasConversion(
				y => JsonConvert.SerializeObject(InventoryModule.Convert(y)),
				y => InventoryModule.Convert(JsonConvert.DeserializeObject<DatabaseInventoryModel>(y)!)).HasColumnType("longtext");
			builder.Property(x => x.LaboratoryOutput).HasColumnName("lab_output").HasConversion(
				y => JsonConvert.SerializeObject(InventoryModule.Convert(y)),
				y => InventoryModule.Convert(JsonConvert.DeserializeObject<DatabaseInventoryModel>(y)!)).HasColumnType("longtext");
			builder.Property(x => x.Starvation).HasColumnName("starvation").HasColumnType("int(11)");
			builder.Property(x => x.Hydration).HasColumnName("hydration").HasColumnType("int(11)");
			builder.Property(x => x.Cuffed).HasColumnName("cuffed").HasColumnType("tinyint(1)");
			builder.Property(x => x.Roped).HasColumnName("roped").HasColumnType("tinyint(1)");
			builder.Property(x => x.Stabilized).HasColumnName("stabilized").HasColumnType("tinyint(1)");
			builder.Property(x => x.WardrobeClothes).HasColumnName("wardrobe").HasConversion(
				y => JsonConvert.SerializeObject(y),
				y => JsonConvert.DeserializeObject<List<ClothesShopItemModel>>(y)!).HasColumnType("longtext");
			builder.Property(x => x.WorkstationId).HasColumnName("workstation").HasColumnType("int(11)");
            builder.Property(x => x.WorkstationItems).HasColumnName("workstation_items").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<List<PlayerWorkstationItemModel>>(y)!).HasColumnType("longtext");
            builder.Property(x => x.TransactionHistory).HasColumnName("transaction_history").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<List<TransactionHistoryModel>>(y)!).HasColumnType("longtext");
			builder.Property(x => x.Duty).HasColumnName("duty").HasColumnType("tinyint(1)");
            builder.Property(x => x.SwatDuty).HasColumnName("swat_duty").HasColumnType("tinyint(1)");
            builder.Property(x => x.SadDuty).HasColumnName("sad_duty").HasColumnType("tinyint(1)");
            builder.Property(x => x.SocialCredit).HasColumnName("social_credit").HasColumnType("int(11)");
			builder.Property(x => x.Injury).HasColumnName("injury").HasColumnType("int(11)");
			builder.Property(x => x.Laptop).HasColumnName("laptop").HasColumnType("tinyint(1)");
			builder.Property(x => x.PlayerCachedClothes).HasColumnName("cached_clothes").HasConversion(
				y => JsonConvert.SerializeObject(y),
				y => JsonConvert.DeserializeObject<ClothesModel>(y)!).HasColumnType("longtext");
			builder.Property(x => x.Strength).HasColumnName("strength").HasColumnType("int(11)");
			builder.Property(x => x.Crimes).HasColumnName("crimes").HasConversion(
				y => JsonConvert.SerializeObject(y),
				y => JsonConvert.DeserializeObject<List<PlayerCrimeModel>>(y)!).HasColumnType("longtext");
			builder.Property(x => x.FederalRecordTeam).HasColumnName("fed_record_team").HasColumnType("varchar(255)");
			builder.Property(x => x.FederalRecordDescription).HasColumnName("fed_record_description").HasColumnType("longtext");
            builder.Property(x => x.FederalRecordPhone).HasColumnName("fed_record_phone").HasColumnType("int(11)");
            builder.Property(x => x.Jailtime).HasColumnName("jail_time").HasColumnType("int(11)");
            builder.Property(x => x.FavoriteAnimations).HasColumnName("favorite_animations").HasConversion(
                 y=> JsonConvert.SerializeObject(y),
                 y => JsonConvert.DeserializeObject<List<AnimationDataModel>>(y)!).HasColumnType("longtext");
			builder.Property(x => x.PhoneBackground).HasColumnName("phone_bg").HasColumnType("int(16)");
		}
	}
}