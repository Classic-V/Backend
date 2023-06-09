using Backend.Utils.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Utils.Configurations;

public class ClothesShopItemModelConfiguration : IEntityTypeConfiguration<ClothesShopItemModel>
{
    public void Configure(EntityTypeBuilder<ClothesShopItemModel> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("server_clothes_shops_items");
        builder.HasIndex(x => x.Id).HasDatabaseName("id");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(11)");
        builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)");
        builder.Property(x => x.Component).HasColumnName("component").HasColumnType("int(11)");
        builder.Property(x => x.Drawable).HasColumnName("drawable").HasColumnType("int(11)");
        builder.Property(x => x.Texture).HasColumnName("texture").HasColumnType("int(11)");
        builder.Property(x => x.Dlc).HasColumnName("dlc").HasColumnType("varchar(255)");
        builder.Property(x => x.Price).HasColumnName("price").HasColumnType("int(11)");
        builder.Property(x => x.IsProp).HasColumnName("is_prop").HasColumnType("tinyint(1)");
        builder.Property(x => x.Gender).HasColumnName("gender").HasColumnType("tinyint(1)");
        builder.Property(x => x.Type).HasColumnName("type").HasColumnType("int(11)");
    }
}