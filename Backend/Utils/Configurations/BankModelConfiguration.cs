using AltV.Net.Data;
using Backend.Utils.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Backend.Utils.Configurations;

public class BankModelConfiguration : IEntityTypeConfiguration<BankModel>
{
    public void Configure(EntityTypeBuilder<BankModel> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("server_banks");
        builder.HasIndex(x => x.Id).HasDatabaseName("id");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(11)");
        builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)");
        builder.Property(x => x.Position).HasColumnName("position").HasConversion(
            y => JsonConvert.SerializeObject(y),
            y => JsonConvert.DeserializeObject<Position>(y)!).HasColumnType("varchar(255)");
        builder.Property(x => x.Type).HasColumnName("type").HasColumnType("int(11)");
    }
}