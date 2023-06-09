using AltV.Net.Data;
using Backend.Utils.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Backend.Utils.Configurations;

public class WardrobeModelConfiguration : IEntityTypeConfiguration<WardrobeModel>
{
    public void Configure(EntityTypeBuilder<WardrobeModel> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("server_wardrobes");
        builder.HasIndex(x => x.Id).HasDatabaseName("id");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(11)");
        builder.Property(x => x.Scale).HasColumnName("scale").HasColumnType("float");
        builder.Property(x => x.Position).HasColumnName("position").HasConversion(
            y => JsonConvert.SerializeObject(y),
            y => JsonConvert.DeserializeObject<Position>(y)).HasColumnType("longtext");
        builder.Property(x => x.Type).HasColumnName("type").HasColumnType("int(11)");
        builder.Property(x => x.Dimension).HasColumnName("dimension").HasColumnType("int(11)");
    }
}