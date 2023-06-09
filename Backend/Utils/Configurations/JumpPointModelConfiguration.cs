
using Backend.Utils.Models.Database;
using Backend.Utils.Models.JumpPoint;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Backend.Utils.Configurations
{
    public class JumpPointModelConfiguration : IEntityTypeConfiguration<JumpPointModel>
    {
        public void Configure(EntityTypeBuilder<JumpPointModel> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("jump_points");
            builder.HasIndex(x => x.Id).HasDatabaseName("id");
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(16)");
			builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)");
			builder.Property(x => x.EntryPosition).HasColumnName("entry_position").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<JumpPointPosition>(y)!).HasColumnType("longtext");
            builder.Property(x => x.ExitPosition).HasColumnName("exit_position").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<JumpPointPosition>(y)!).HasColumnType("longtext");
            builder.Property(x => x.PlayerAccessList).HasColumnName("player_access_list").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<List<int>>(y)!).HasColumnType("longtext");
            builder.Property(x => x.FactionAccessList).HasColumnName("faction_access_list").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<List<int>>(y)!).HasColumnType("longtext");
            builder.Property(x => x.Ipl).HasColumnName("ipl").HasColumnType("varchar(255)");
			builder.Property(x => x.Locked).HasColumnName("locked").HasColumnType("tinyint(1)");
		}
    }
}
