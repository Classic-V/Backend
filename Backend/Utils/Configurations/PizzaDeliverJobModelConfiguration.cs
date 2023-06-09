using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.PizzaDeliveryJob;
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
    public class PizzaDeliverJobModelConfiguration : IEntityTypeConfiguration<PizzaDeliveryJobModel>
    {
        public void Configure(EntityTypeBuilder<PizzaDeliveryJobModel> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("server_pizza_delivery_job");
            builder.HasIndex(x => x.Id).HasDatabaseName("id");
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(11)");
            builder.Property(x => x.VehiclePosition).HasColumnName("vehicle_position").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<PositionModel>(y)!).HasColumnType("longtext");
            builder.Property(x => x.Deliveries).HasColumnName("deliveries").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<List<DeliveryReturnData>>(y)!).HasColumnType("longtext");
            builder.Property(x => x.MaxDeliveries).HasColumnName("max_deliveries").HasColumnType("int(11)");
            builder.Property(x => x.MaxPrice).HasColumnName("max_price").HasColumnType("int(11)");
        }
    }
}
