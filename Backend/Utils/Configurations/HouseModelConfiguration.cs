using AltV.Net.Data;
using Backend.Modules.Inventory;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.House;
using Backend.Utils.Models.Inventory.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Backend.Utils.Configurations;

public class HouseModelConfiguration : IEntityTypeConfiguration<HouseModel>
{
    public void Configure(EntityTypeBuilder<HouseModel> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("server_houses");
        builder.HasIndex(x => x.Id).HasDatabaseName("id");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(11)");
        builder.Property(x => x.Owner).HasColumnName("owner_id").HasColumnType("int(11)");
        builder.Property(x => x.Type).HasColumnName("type").HasColumnType("int(11)");
        builder.Property(x => x.Position).HasColumnName("position").HasConversion(
            y => JsonConvert.SerializeObject(y),
            y => JsonConvert.DeserializeObject<Position>(y)!).HasColumnType("longtext");
        builder.Property(x => x.RentMembers).HasColumnName("rent_members").HasConversion(
            y => JsonConvert.SerializeObject(y),
            y => JsonConvert.DeserializeObject<List<RentMemberData>>(y)!).HasColumnType("longtext");
        builder.Property(x => x.Price).HasColumnName("price").HasColumnType("int(16)");
        builder.Property(x => x.Inventory).HasColumnName("inventory").HasConversion(
            y => JsonConvert.SerializeObject(InventoryModule.Convert(y)),
            y => InventoryModule.Convert(JsonConvert.DeserializeObject<DatabaseInventoryModel>(y)!)).HasColumnType("longtext");
        builder.Property(x => x.WardrobeId).HasColumnName("wardrobe_id").HasColumnType("int(11)");

    }
}