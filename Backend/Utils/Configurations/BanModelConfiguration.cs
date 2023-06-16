
using Backend.Utils.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Utils.Configurations
{
	public class BanModelConfiguration : IEntityTypeConfiguration<BanModel>
	{
		public void Configure(EntityTypeBuilder<BanModel> builder)
		{
			builder.HasKey(x => x.Id);
			builder.ToTable("bans");
			builder.HasIndex(x => x.Id).HasDatabaseName("id");
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(16)");
            builder.Property(x => x.AccountId).HasColumnName("accountId").HasColumnType("int(16)");
            builder.Property(x => x.Reason).HasColumnName("reason").HasColumnType("varchar(255)");
            builder.Property(x => x.Active).HasColumnName("active").HasColumnType("tinyint(1)");
            builder.Property(x => x.Permanent).HasColumnName("permanent").HasColumnType("tinyint(1)");
            builder.Property(x => x.BannedFrom).HasColumnName("bannedFrom").HasColumnType("datetime");
            builder.Property(x => x.BannedTill).HasColumnName("bannedTill").HasColumnType("datetime");
		}
	}
}