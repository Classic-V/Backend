using AltV.Net.Data;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Door;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Backend.Utils.Configurations
{
    public class DoorModelConfiguration : IEntityTypeConfiguration<DoorModel>
    {
        public void Configure(EntityTypeBuilder<DoorModel> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("server_doors");
            builder.HasIndex(x => x.Id).HasDatabaseName("id");
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(11)");
            builder.Property(x => x.Position).HasColumnName("position").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<Position>(y)).HasColumnType("longtext");
            builder.Property(x => x.Doors).HasColumnName("doors").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<List<DoorData>>(y)!).HasColumnType("longtext");
            builder.Property(x => x.Type).HasColumnName("type").HasColumnType("int(11)");
            builder.Property(x => x.Locked).HasColumnName("locked").HasColumnType("tinyint(1)");
            builder.Property(x => x.Radius).HasColumnName("radius").HasColumnType("float");
            builder.Property(x => x.Access).HasColumnName("access").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<List<int>>(y)!).HasColumnType("longtext");
        }
    }
}
