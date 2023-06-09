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
    public class BarberItemModelConfiguration : IEntityTypeConfiguration<BarberItemModel>
    {
        public void Configure(EntityTypeBuilder<BarberItemModel> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("server_barbers_items");
            builder.HasIndex(x => x.Id).HasDatabaseName("id");
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(11)");
            builder.Property(x => x.ShopId).HasColumnName("shop_id").HasColumnType("int(11)");
            builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)");
            builder.Property(x => x.DrawableId).HasColumnName("drawable_id").HasColumnType("int(11)");
            builder.Property(x => x.Price).HasColumnName("price").HasColumnType("int(11)");
        }
    }
}
