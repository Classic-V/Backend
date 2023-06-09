using Backend.Utils.Models.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AltV.Net.Data;

namespace Backend.Utils.Configurations
{
	public class BlipModelConfiguration : IEntityTypeConfiguration<BlipModel>
	{
		public void Configure(EntityTypeBuilder<BlipModel> builder)
		{
			builder.HasKey(x => x.Id);
			builder.ToTable("blips");
			builder.HasIndex(x => x.Id).HasDatabaseName("id");
			builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(16)");
			builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)");
			builder.Property(x => x.Position).HasColumnName("position").HasConversion(
				y => JsonConvert.SerializeObject(y),
				y => JsonConvert.DeserializeObject<Position>(y)!).HasColumnType("longtext");
			builder.Property(x => x.Sprite).HasColumnName("sprite").HasColumnType("int(16)");
			builder.Property(x => x.Color).HasColumnName("color").HasColumnType("int(16)");
			builder.Property(x => x.ShortRange).HasColumnName("short_range").HasColumnType("tinyint(1)");
		}
	}
}