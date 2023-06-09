using Backend.Utils.Models.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Backend.Utils.Models;
using Backend.Utils.Models.Vehicle;
using Newtonsoft.Json;
using Backend.Utils.Models.Inventory.Database;
using Backend.Modules.Inventory;

namespace Backend.Utils.Configurations
{
	public class VehicleModelConfiguration : IEntityTypeConfiguration<VehicleModel>
	{
		public void Configure(EntityTypeBuilder<VehicleModel> builder)
		{
			builder.HasKey(x => x.Id);
			builder.ToTable("vehicles");
			builder.HasIndex(x => x.Id).HasDatabaseName("id");
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(16)");
			builder.Property(x => x.InfoModelId).HasColumnName("model_id").HasColumnType("int(32)");
			builder.Property(x => x.Tuning).HasColumnName("tuning").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<TuningModel>(y)!).HasColumnType("longtext");
            builder.Property(x => x.Owner).HasColumnName("owner").HasColumnType("int(32)");
			builder.Property(x => x.Parked).HasColumnName("parked").HasColumnType("tinyint(1)");
			builder.Property(x => x.Garage).HasColumnName("garage").HasColumnType("int(32)");
			builder.Property(x => x.Position).HasColumnName("position").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<PositionModel>(y)!).HasColumnType("longtext");
            builder.Property(x => x.Trunk).HasColumnName("trunk").HasConversion(
				y => JsonConvert.SerializeObject(InventoryModule.Convert(y)),
				y => InventoryModule.Convert(JsonConvert.DeserializeObject<DatabaseInventoryModel>(y)!)).HasColumnType("longtext");
			builder.Property(x => x.GloveBox).HasColumnName("glove_box").HasConversion(
				y => JsonConvert.SerializeObject(InventoryModule.Convert(y)),
				y => InventoryModule.Convert(JsonConvert.DeserializeObject<DatabaseInventoryModel>(y)!)).HasColumnType("longtext");
			builder.Property(x => x.Type).HasColumnName("type").HasColumnType("int(16)");
			builder.Property(x => x.Plate).HasColumnName("plate").HasColumnType("varchar(64)");
			builder.Property(x => x.Note).HasColumnName("note").HasColumnType("varchar(64)");
			builder.Property(x => x.Fuel).HasColumnName("fuel").HasColumnType("float");
		}
	}
}