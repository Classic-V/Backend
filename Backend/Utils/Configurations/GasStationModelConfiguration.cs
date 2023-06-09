using Backend.Utils.Models.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AltV.Net.Data;

namespace Backend.Utils.Configurations
{
	public class GasStationModelConfiguration : IEntityTypeConfiguration<GasStationModel>
	{
		public void Configure(EntityTypeBuilder<GasStationModel> builder)
		{
			builder.HasKey(x => x.Id);
			builder.ToTable("gas_stations");
			builder.HasIndex(x => x.Id).HasDatabaseName("id");
			builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(16)");
			builder.Property(x => x.Position).HasColumnName("position").HasConversion(
				y => JsonConvert.SerializeObject(y),
				y => JsonConvert.DeserializeObject<Position>(y)!).HasColumnType("longtext");
		}
	}
}