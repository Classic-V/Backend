using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net.Data;
using Backend.Modules.Inventory;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Inventory.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Backend.Utils.Configurations
{
    public class StorageModelConfiguration : IEntityTypeConfiguration<StorageModel>
    {
        public void Configure(EntityTypeBuilder<StorageModel> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("storages");
            builder.HasIndex(x => x.Id).HasDatabaseName("id");
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(32)");
            builder.Property(x => x.OwnerId).HasColumnName("owner_id").HasColumnType("int(32)");
			builder.Property(x => x.WarehouseType).HasColumnName("type").HasColumnType("int(32)");
			builder.Property(x => x.Position).HasColumnName("position").HasConversion(
				y => JsonConvert.SerializeObject(y),
				y => JsonConvert.DeserializeObject<Position>(y)!).HasColumnType("longtext");
            builder.Property(x => x.Inventories).HasColumnName("inventories").HasConversion(
				y => JsonConvert.SerializeObject(InventoryModule.Convert(y)),
				y => InventoryModule.Convert(JsonConvert.DeserializeObject<List<DatabaseInventoryModel>>(y)!)).HasColumnType("longtext");
        }
    }
}
