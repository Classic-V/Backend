using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Backend.Utils.Models.Database;
using Newtonsoft.Json;
using AltV.Net.Data;
using Backend.Utils.Models;
using Backend.Utils.Models.VehicleShop;

namespace Backend.Utils.Configurations
{
	public class VehicleShopModelConfiguration : IEntityTypeConfiguration<VehicleShopModel>
	{
		public void Configure(EntityTypeBuilder<VehicleShopModel> builder)
		{
			builder.HasKey(x => x.Id);
			builder.ToTable("vehicle_shops");
			builder.HasIndex(x => x.Id).HasDatabaseName("id");
			builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(16)");
			builder.Property(x => x.Position).HasColumnName("position").HasConversion(
				y => JsonConvert.SerializeObject(y),
				y => JsonConvert.DeserializeObject<Position>(y)!).HasColumnType("longtext");
			builder.Property(x => x.Ped).HasColumnName("ped").HasConversion(
				y => JsonConvert.SerializeObject(y),
				y => JsonConvert.DeserializeObject<PositionModel>(y)!).HasColumnType("longtext");
			builder.Property(x => x.Vehicles).HasColumnName("vehicles").HasConversion(
				y => JsonConvert.SerializeObject(y),
				y => JsonConvert.DeserializeObject<List<VehicleShopItem>>(y)!).HasColumnType("longtext");
			builder.Property(x => x.Spawns).HasColumnName("spawns").HasConversion(
				y => JsonConvert.SerializeObject(y),
				y => JsonConvert.DeserializeObject<List<PositionModel>>(y)!).HasColumnType("longtext");
		}
	}
}