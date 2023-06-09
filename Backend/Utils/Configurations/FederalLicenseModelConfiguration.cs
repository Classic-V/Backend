using Backend.Utils.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Configurations
{
    public class FederalLicenseModelConfiguration : IEntityTypeConfiguration<FederalLicenseModel>
    {
        public void Configure(EntityTypeBuilder<FederalLicenseModel> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("server_federal_license");
            builder.HasIndex(x => x.Id).HasDatabaseName("id");
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(11)");
            builder.Property(x => x.PlayerId).HasColumnName("player_id").HasColumnType("int(11)");
            builder.Property(x => x.GpsFindLicense).HasColumnName("gps_find_license").HasColumnType("tinyint(1)");
            builder.Property(x => x.PhoneHistoryLicense).HasColumnName("phone_history_license").HasColumnType("tinyint(1)");
            builder.Property(x => x.SadLicense).HasColumnName("sad_license").HasColumnType("tinyint(1)");
        }
    }
}
