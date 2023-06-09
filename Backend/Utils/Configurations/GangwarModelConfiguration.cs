using AltV.Net.Data;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Gangwar;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Backend.Utils.Configurations
{
    public class GangwarModelConfiguration : IEntityTypeConfiguration<GangwarModel>
    {
        public void Configure(EntityTypeBuilder<GangwarModel> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("gangwars");
            builder.HasIndex(x => x.Id).HasDatabaseName("id");
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(16)");
            builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)");
            builder.Property(x => x.OwnerId).HasColumnName("owner_id").HasColumnType("int(16)");
            builder.Property(x => x.AttackPosition).HasColumnName("attack_position").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<Position>(y)).HasColumnType("longtext");
            builder.Property(x => x.Radius).HasColumnName("radius").HasColumnType("float");
            builder.Property(x => x.Flags).HasColumnName("flags").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<List<FlagModel>>(y)!).HasColumnType("longtext");
            builder.Property(x => x.LastAttacked).HasColumnName("last_attacked").HasColumnType("datetime");
            builder.Property(x => x.AttackerSpawnPosition).HasColumnName("attacker_spawn_position").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<PositionModel>(y)!).HasColumnType("longtext");
            builder.Property(x => x.DefenderSpawnPosition).HasColumnName("defender_spawn_position").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<PositionModel>(y)!).HasColumnType("longtext");
            builder.Property(x => x.AttackerVehicleSpawnPosition).HasColumnName("attacker_vehicle_spawn_position").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<List<PositionModel>>(y)!).HasColumnType("longtext");
            builder.Property(x => x.DefenderVehicleSpawnPosition).HasColumnName("defender_vehicle_spawn_position").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<List<PositionModel>>(y)!).HasColumnType("longtext");
        }
    }
}
