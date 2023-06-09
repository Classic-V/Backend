using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Backend.Utils.Models.Database;

namespace Backend.Utils.Configurations
{
	public class VehicleInfoModelConfiguration : IEntityTypeConfiguration<VehicleInfoModel>
	{
		public void Configure(EntityTypeBuilder<VehicleInfoModel> builder)
		{
			builder.HasKey(x => x.Id);
			builder.ToTable("vehicle_info");
			builder.HasIndex(x => x.Id).HasDatabaseName("id");
			builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(16)");
			builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)");
			builder.Property(x => x.Hash).HasColumnName("hash").HasColumnType("int(32)");
			builder.Property(x => x.TrunkWeight).HasColumnName("trunk_weight").HasColumnType("int(16)");
			builder.Property(x => x.TrunkSlots).HasColumnName("trunk_slots").HasColumnType("int(16)");
			builder.Property(x => x.GloveBoxWeight).HasColumnName("glove_box_weight").HasColumnType("int(16)");
			builder.Property(x => x.GloveBoxSlots).HasColumnName("glove_box_slots").HasColumnType("int(16)");
			builder.Property(x => x.MaxFuel).HasColumnName("max_fuel").HasColumnType("int(16)");
			builder.Property(x => x.GarageType).HasColumnName("garage_type").HasColumnType("int(16)");
			builder.Property(x => x.Tax).HasColumnName("tax").HasColumnType("int(16)");
			builder.Property(x => x.Seats).HasColumnName("seats").HasColumnType("int(16)");
		}
	}
}