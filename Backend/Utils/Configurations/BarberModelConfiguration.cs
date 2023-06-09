using AltV.Net.Data;
using Backend.Utils.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Configurations
{
    public class BarberModelConfiguration : IEntityTypeConfiguration<BarberModel>
    {
        public void Configure(EntityTypeBuilder<BarberModel> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("server_barbers");
            builder.HasIndex(x => x.Id).HasDatabaseName("id");
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(11)");
            builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)");
            builder.Property(x => x.Position).HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<Position>(y)!).HasColumnName("position").HasColumnType("longtext");
            builder.Property(x => x.Radius).HasColumnName("radius").HasColumnType("float");
        }
    }
}
