using AltV.Net.Data;
using Backend.Modules.Inventory;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Farming;
using Backend.Utils.Models.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Configurations
{
	public class FarmingConfiguration : IEntityTypeConfiguration<FarmingModel>
	{
		public void Configure(EntityTypeBuilder<FarmingModel> builder)
		{
			builder.HasKey(x => x.Id);
			builder.ToTable("farming_spots");
			builder.HasIndex(x => x.Id).HasDatabaseName("id");
			builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(16)");
			builder.Property(x => x.Position).HasColumnName("position").HasConversion(
				y => JsonConvert.SerializeObject(y),
				y => JsonConvert.DeserializeObject<Position>(y)!).HasColumnType("longtext");
			builder.Property(x => x.Animation).HasColumnName("anim").HasColumnType("int(32)");
			builder.Property(x => x.ObjectHash).HasColumnName("object_hash").HasColumnType("int(32)");
			builder.Property(x => x.Objects).HasColumnName("objects").HasConversion(
				y => JsonConvert.SerializeObject(y),
				y => JsonConvert.DeserializeObject<List<FarmingObjectModel>>(y)!).HasColumnType("longtext");
			builder.Property(x => x.NeededItem).HasColumnName("needed_item").HasColumnType("int(32)");
			builder.Property(x => x.GetItem).HasColumnName("get_item").HasColumnType("int(32)");
			builder.Property(x => x.MinItemGet).HasColumnName("min_item_get").HasColumnType("int(32)");
			builder.Property(x => x.MaxItemGet).HasColumnName("max_item_get").HasColumnType("int(32)");
		}
	}
}