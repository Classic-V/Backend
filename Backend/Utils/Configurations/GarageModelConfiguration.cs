using Backend.Utils.Models.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Utils.Models;
using Newtonsoft.Json;
using AltV.Net.Data;

namespace Backend.Utils.Configurations
{
	public class GarageModelConfiguration : IEntityTypeConfiguration<GarageModel>
	{
		public void Configure(EntityTypeBuilder<GarageModel> builder)
		{
			builder.HasKey(x => x.Id);
			builder.ToTable("garages");
			builder.HasIndex(x => x.Id).HasDatabaseName("id");
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(16)");
			builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)");
			builder.Property(x => x.Position).HasColumnName("position").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<Position>(y)!).HasColumnType("longtext");
			builder.Property(x => x.PedPosition).HasColumnName("ped").HasConversion(
				y => JsonConvert.SerializeObject(y),
				y => JsonConvert.DeserializeObject<PositionModel>(y)!).HasColumnType("longtext");
			builder.Property(x => x.Spawns).HasColumnName("spawns").HasConversion(
                y => JsonConvert.SerializeObject(y),
                y => JsonConvert.DeserializeObject<List<PositionModel>>(y)!).HasColumnType("longtext");
            builder.Property(x => x.Type).HasColumnName("type").HasColumnType("int(16)");
			builder.Property(x => x.Owner).HasColumnName("owner").HasColumnType("int(16)");
			builder.Property(x => x.VehicleType).HasColumnName("veh_type").HasColumnType("int(16)");
		}
	}
}