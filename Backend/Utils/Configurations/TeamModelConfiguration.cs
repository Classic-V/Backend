using Backend.Utils.Models.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Utils.Models;
using Backend.Utils.Models.Inventory.Database;
using Backend.Utils.Models.Vehicle;
using Newtonsoft.Json;
using Backend.Modules.Inventory;
using Backend.Utils.Models.Team.Client;
using AltV.Net.Data;
using Backend.Utils.Models.Team;

namespace Backend.Utils.Configurations
{
	public class TeamModelConfiguration : IEntityTypeConfiguration<TeamModel>
	{
		public void Configure(EntityTypeBuilder<TeamModel> builder)
		{
			builder.HasKey(x => x.Id);
			builder.ToTable("teams");
			builder.HasIndex(x => x.Id).HasDatabaseName("id");
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(16)");
			builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)");
			builder.Property(x => x.ShortName).HasColumnName("short_name").HasColumnType("varchar(64)");
			builder.Property(x => x.Position).HasColumnName("position").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<Position>(y)!).HasColumnType("longtext");
            builder.Property(x => x.Color).HasColumnName("color").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<RgbModel>(y)!).HasColumnType("longtext");
            builder.Property(x => x.BlipColor).HasColumnName("blip_color").HasColumnType("int(16)");
			builder.Property(x => x.Type).HasColumnName("type").HasColumnType("int(16)");
			builder.Property(x => x.Warns).HasColumnName("warns").HasColumnType("int(16)");
			builder.Property(x => x.MeeleWeapon).HasColumnName("meele").HasColumnType("varchar(255)");
			builder.Property(x => x.MeeleWeaponHash).HasColumnName("meele_hash").HasColumnType("int(32)");
			builder.Property(x => x.Drug).HasColumnName("drug").HasColumnType("int(32)");
			builder.Property(x => x.ProcessedDrug).HasColumnName("processed_drug").HasColumnType("int(32)");
			builder.Property(x => x.Money).HasColumnName("money").HasColumnType("int(32)");
			builder.Property(x => x.BankHistory).HasColumnName("bank_history").HasConversion(
				y => JsonConvert.SerializeObject(y),
				y => JsonConvert.DeserializeObject<List<TeamClientBankData>>(y)!).HasColumnType("longtext");
			builder.Property(x => x.Laboratory).HasColumnName("lab").HasConversion(
				y => JsonConvert.SerializeObject(y),
				y => JsonConvert.DeserializeObject<Position>(y)!).HasColumnType("longtext");
			builder.Property(x => x.LabFuel).HasColumnName("lab_fuel").HasConversion(
				y => JsonConvert.SerializeObject(InventoryModule.Convert(y)),
				y => InventoryModule.Convert(JsonConvert.DeserializeObject<DatabaseInventoryModel>(y)!)).HasColumnType("longtext");
			builder.Property(x => x.Inventories).HasColumnName("inventories").HasConversion(
				y => JsonConvert.SerializeObject(TeamModel.ConvertInventories(y)),
				y => TeamModel.ConvertInventories(JsonConvert.DeserializeObject<List<TeamDatabaseInventoryModel>>(y)!)).HasColumnType("longtext");
		}
	}
}