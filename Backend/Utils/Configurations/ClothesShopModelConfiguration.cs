using AltV.Net.Data;using Backend.Utils.Models;
using Backend.Utils.Models.ClothesShop;
using Backend.Utils.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Backend.Utils.Configurations;

public class ClothesShopModelConfiguration : IEntityTypeConfiguration<ClothesShopModel>
{
    public void Configure(EntityTypeBuilder<ClothesShopModel> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("server_clothes_shops");
        builder.HasIndex(x => x.Id).HasDatabaseName("id");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(11)");
        builder.Property(x => x.Position).HasColumnName("position").HasConversion(
            y => JsonConvert.SerializeObject(y),
            y => JsonConvert.DeserializeObject<PositionModel>(y)!).HasColumnType("longtext");
		builder.Property(x => x.Type).HasColumnName("type").HasColumnType("int(11)");
	}
}