using AltV.Net.Data;
using Backend.Utils.Models;
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
	public class ProcessorConfiguration : IEntityTypeConfiguration<ProcessorModel>
	{
		public void Configure(EntityTypeBuilder<ProcessorModel> builder)
		{
			builder.HasKey(x => x.Id);
			builder.ToTable("processors");
			builder.HasIndex(x => x.Id).HasDatabaseName("id");
			builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(16)");
			builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)");
			builder.Property(x => x.Position).HasColumnName("position").HasConversion(
				y => JsonConvert.SerializeObject(y),
				y => JsonConvert.DeserializeObject<Position>(y)!).HasColumnType("longtext");
			builder.Property(x => x.Ped).HasColumnName("ped").HasConversion(
				y => JsonConvert.SerializeObject(y),
				y => JsonConvert.DeserializeObject<PositionModel>(y)!).HasColumnType("longtext");
			builder.Property(x => x.NeededItem).HasColumnName("needed_item").HasColumnType("int(32)");
			builder.Property(x => x.GetItem).HasColumnName("get_item").HasColumnType("int(32)");
			builder.Property(x => x.NeededItemAmount).HasColumnName("needed_item_amount").HasColumnType("int(32)");
			builder.Property(x => x.GetItemAmount).HasColumnName("get_item_amount").HasColumnType("int(32)");
			builder.Property(x => x.Duration).HasColumnName("duration").HasColumnType("int(32)");
		}
	}
}