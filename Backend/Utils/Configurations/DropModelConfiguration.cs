using Backend.Utils.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Utils.Models;
using Newtonsoft.Json;
using AltV.Net.Data;

namespace Backend.Utils.Configurations
{
	public class DropModelConfiguration : IEntityTypeConfiguration<DropModel>
	{
		public void Configure(EntityTypeBuilder<DropModel> builder)
		{
			builder.HasKey(x => x.Id);
			builder.ToTable("drops");
			builder.HasIndex(x => x.Id).HasDatabaseName("id");
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(16)");
			builder.Property(x => x.Location).HasColumnName("location").HasColumnType("varchar(255)");
            builder.Property(x => x.Position).HasColumnName("position").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<PositionModel>(y)!).HasColumnType("longtext");
			builder.Property(x => x.LootCrate1).HasColumnName("loot_crate_1").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<PositionModel>(y)!).HasColumnType("longtext");
            builder.Property(x => x.LootCrate2).HasColumnName("loot_crate_2").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<PositionModel>(y)!).HasColumnType("longtext");
			builder.Property(x => x.LootCrate3).HasColumnName("loot_crate_3").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<PositionModel>(y)!).HasColumnType("longtext");
        }
	}
}