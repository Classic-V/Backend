using Backend.Utils.Models.ClothesShop;
using Backend.Utils.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Backend.Utils.Configurations;

public class OutfitModelConfiguration : IEntityTypeConfiguration<OutfitModel>
{
    public void Configure(EntityTypeBuilder<OutfitModel> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("player_outfits");
        builder.HasIndex(x => x.Id).HasDatabaseName("id");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(11)");
        builder.Property(x => x.PlayerId).HasColumnName("player_id").HasColumnType("int(11)");
        builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)");
        builder.Property(x => x.Masks).HasColumnName("masks").HasConversion(
            y => JsonConvert.SerializeObject(y),
            y => JsonConvert.DeserializeObject<ClothingModel>(y)!).HasColumnType("longtext");
        builder.Property(x => x.Torsos).HasColumnName("torsos").HasConversion(
            y => JsonConvert.SerializeObject(y),
            y => JsonConvert.DeserializeObject<ClothingModel>(y)!).HasColumnType("longtext");
        builder.Property(x => x.Legs).HasColumnName("legs").HasConversion(
            y => JsonConvert.SerializeObject(y),
            y => JsonConvert.DeserializeObject<ClothingModel>(y)!).HasColumnType("longtext");
        builder.Property(x => x.Bags).HasColumnName("bags").HasConversion(
            y => JsonConvert.SerializeObject(y),
            y => JsonConvert.DeserializeObject<ClothingModel>(y)!).HasColumnType("longtext");
        builder.Property(x => x.Shoes).HasColumnName("shoes").HasConversion(
            y => JsonConvert.SerializeObject(y),
            y => JsonConvert.DeserializeObject<ClothingModel>(y)!).HasColumnType("longtext");
        builder.Property(x => x.Accessories).HasColumnName("accessories").HasConversion(
            y => JsonConvert.SerializeObject(y),
            y => JsonConvert.DeserializeObject<ClothingModel>(y)!).HasColumnType("longtext");
        builder.Property(x => x.Undershirts).HasColumnName("undershirts").HasConversion(
            y => JsonConvert.SerializeObject(y),
            y => JsonConvert.DeserializeObject<ClothingModel>(y)!).HasColumnType("longtext");
        builder.Property(x => x.Decals).HasColumnName("decals").HasConversion(
            y => JsonConvert.SerializeObject(y),
            y => JsonConvert.DeserializeObject<ClothingModel>(y)!).HasColumnType("longtext");
        builder.Property(x => x.Tops).HasColumnName("tops").HasConversion(
            y => JsonConvert.SerializeObject(y),
            y => JsonConvert.DeserializeObject<ClothingModel>(y)!).HasColumnType("longtext");
        builder.Property(x => x.Hats).HasColumnName("hats").HasConversion(
            y => JsonConvert.SerializeObject(y),
            y => JsonConvert.DeserializeObject<ClothingModel>(y)!).HasColumnType("longtext");
        builder.Property(x => x.Glasses).HasColumnName("glasses").HasConversion(
            y => JsonConvert.SerializeObject(y),
            y => JsonConvert.DeserializeObject<ClothingModel>(y)!).HasColumnType("longtext");
        builder.Property(x => x.Ears).HasColumnName("ears").HasConversion(
            y => JsonConvert.SerializeObject(y),
            y => JsonConvert.DeserializeObject<ClothingModel>(y)!).HasColumnType("longtext");
        builder.Property(x => x.Watches).HasColumnName("watches").HasConversion(
            y => JsonConvert.SerializeObject(y),
            y => JsonConvert.DeserializeObject<ClothingModel>(y)!).HasColumnType("longtext");
        builder.Property(x => x.Bracelets).HasColumnName("bracelets").HasConversion(
            y => JsonConvert.SerializeObject(y),
            y => JsonConvert.DeserializeObject<ClothingModel>(y)!).HasColumnType("longtext");
    }
}