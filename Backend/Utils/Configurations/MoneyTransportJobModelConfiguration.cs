using AltV.Net.Data;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.MoneyTransportJob;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Backend.Utils.Configurations
{
    public class MoneyTransportJobModelConfiguration : IEntityTypeConfiguration<MoneyTransportJobModel>
    {
        public void Configure(EntityTypeBuilder<MoneyTransportJobModel> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("server_money_transport_job");
            builder.HasIndex(x => x.Id).HasDatabaseName("id");
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(11)");
            builder.Property(x => x.VehiclePosition).HasColumnName("vehicle_position").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<PositionModel>(y)!).HasColumnType("longtext");
            builder.Property(x => x.Ids).HasColumnName("ids").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<List<BankReturnData>>(y)!).HasColumnType("longtext");
            builder.Property(x => x.MaxPrice).HasColumnName("max_price").HasColumnType("int(11)");
        }
    }
}
