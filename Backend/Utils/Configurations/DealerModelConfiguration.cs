using Backend.Utils.Models.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Backend.Utils.Models;

namespace Backend.Utils.Configurations
{
	public class DealerModelConfiguration : IEntityTypeConfiguration<DealerModel>
	{
		public void Configure(EntityTypeBuilder<DealerModel> builder)
		{
			builder.HasKey(x => x.Id);
			builder.ToTable("dealer");
			builder.HasIndex(x => x.Id).HasDatabaseName("id");
			builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(16)");
			builder.Property(x => x.Position).HasColumnName("position").HasConversion(
				y => JsonConvert.SerializeObject(y),
				y => JsonConvert.DeserializeObject<PositionModel>(y)!).HasColumnType("longtext");
		}
	}
}