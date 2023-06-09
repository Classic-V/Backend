using AltV.Net.Data;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.PostJob;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Backend.Utils.Configurations;

public class PostJobModelConfiguration : IEntityTypeConfiguration<PostJobModel>
{
    public void Configure(EntityTypeBuilder<PostJobModel> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("server_jobs_post");
        builder.HasIndex(x => x.Id).HasDatabaseName("id");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(11)");
        builder.Property(x => x.VehicleStart).HasColumnName("vehicle_start").HasConversion(
            y => JsonConvert.SerializeObject(y),
            y => JsonConvert.DeserializeObject<PositionModel>(y)!).HasColumnType("longtext");
        builder.Property(x => x.ReturnPoints).HasColumnName("return_points").HasConversion(
            y => JsonConvert.SerializeObject(y),
            y => JsonConvert.DeserializeObject<List<PostReturnData>>(y)!).HasColumnType("longtext");
        builder.Property(x => x.MaxHouses).HasColumnName("max_houses").HasColumnType("int(11)");
        builder.Property(x => x.MaxPrice).HasColumnName("max_price").HasColumnType("int(11)");
    }
}