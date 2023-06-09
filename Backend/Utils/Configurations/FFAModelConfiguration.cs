using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Backend.Utils.Configurations;

public class FFAModelConfiguration : IEntityTypeConfiguration<FFAModel>
{
    public void Configure(EntityTypeBuilder<FFAModel> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("ffas");
        builder.HasIndex(x => x.Id).HasDatabaseName("id");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(16)");
        builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)");
        builder.Property(x => x.Radius).HasColumnName("radius").HasColumnType("float");
        builder.Property(x => x.MaxPlayers).HasColumnName("max_players").HasColumnType("int(16)");
        builder.Property(x => x.Spawns).HasConversion(
            y => JsonConvert.SerializeObject(y),
            y => JsonConvert.DeserializeObject<List<PositionModel>>(y)!).HasColumnName("spawns").HasColumnType("longtext");
        builder.Property(x => x.FirstWeapon).HasColumnName("first_weapon").HasColumnType("int(16)");
        builder.Property(x => x.SecondWeapon).HasColumnName("second_weapon").HasColumnType("int(16)");

    } 
}