using AltV.Net.Data;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Shops;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Backend.Utils.Configurations
{
    public class ShopModelConfiguration : IEntityTypeConfiguration<ShopModel>
    {
        public void Configure(EntityTypeBuilder<ShopModel> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("shops");
            builder.HasIndex(x => x.Id).HasDatabaseName("id");
			builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(11)");
			builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)");
            builder.Property(x => x.Position).HasColumnName("position").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<Position>(y)).HasColumnType("longtext");
			builder.Property(x => x.PedPosition).HasColumnName("ped").HasConversion(
				y => JsonConvert.SerializeObject(y),
				y => JsonConvert.DeserializeObject<PositionModel>(y)!).HasColumnType("longtext");
			builder.Property(x => x.Type).HasColumnName("type").HasColumnType("int(11)");
            builder.Property(x => x.Items).HasColumnName("items").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<List<ShopItemModel>>(y)!).HasColumnType("longtext");
			builder.Property(x => x.OwnerId).HasColumnName("ownerid").HasColumnType("int(11)");
		}
    }
}
