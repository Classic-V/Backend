using AltV.Net.Data;
using Backend.Utils.Models;
using Backend.Utils.Models.Ammunation;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Shops;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Backend.Utils.Configurations;

public class AmmunationModelConfiguration : IEntityTypeConfiguration<AmmunationModel>
{
    public void Configure(EntityTypeBuilder<AmmunationModel> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("ammunation_shops");
        builder.HasIndex(x => x.Id).HasDatabaseName("id");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(11)");
        builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)");
        builder.Property(x => x.Position).HasColumnName("position").HasConversion(
            y => JsonConvert.SerializeObject(y),
            y => JsonConvert.DeserializeObject<Position>(y)).HasColumnType("longtext");
        builder.Property(x => x.PedPosition).HasColumnName("ped").HasConversion(
            y => JsonConvert.SerializeObject(y),
            y => JsonConvert.DeserializeObject<PositionModel>(y)!).HasColumnType("longtext");
        builder.Property(x => x.Items).HasColumnName("items").HasConversion(
            y => JsonConvert.SerializeObject(y),
            y => JsonConvert.DeserializeObject<List<WeaponShopItemModel>>(y)!).HasColumnType("longtext");
    }
}