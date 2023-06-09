using Backend.Utils.Models.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AltV.Net.Data;
using Backend.Utils.Models.Workstation;
using Backend.Utils.Models;

namespace Backend.Utils.Configurations
{
	public class WorkstationModelConfiguration : IEntityTypeConfiguration<WorkstationModel>
	{
		public void Configure(EntityTypeBuilder<WorkstationModel> builder)
		{
			builder.HasKey(x => x.Id);
			builder.ToTable("workstations");
			builder.HasIndex(x => x.Id).HasDatabaseName("id");
			builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(16)");
			builder.Property(x => x.Position).HasColumnName("position").HasConversion(
				y => JsonConvert.SerializeObject(y),
				y => JsonConvert.DeserializeObject<Position>(y)).HasColumnType("longtext");
			builder.Property(x => x.Ped).HasColumnName("ped").HasConversion(
				y => JsonConvert.SerializeObject(y),
				y => JsonConvert.DeserializeObject<PositionModel>(y)!).HasColumnType("longtext");
			builder.Property(x => x.Items).HasColumnName("items").HasConversion(
				y => JsonConvert.SerializeObject(y),
				y => JsonConvert.DeserializeObject<List<WorkstationItemModel>>(y)!).HasColumnType("longtext");
		}
	}
}