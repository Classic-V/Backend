using Backend.Utils.Models.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Configurations
{
	public class CrimeModelConfiguration : IEntityTypeConfiguration<CrimeModel>
	{
		public void Configure(EntityTypeBuilder<CrimeModel> builder)
		{
			builder.HasKey(x => x.Id);
			builder.ToTable("crimes");
			builder.HasIndex(x => x.Id).HasDatabaseName("id");
			builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(16)");
			builder.Property(x => x.Group).HasColumnName("group").HasColumnType("int(16)");
			builder.Property(x => x.Label).HasColumnName("label").HasColumnType("varchar(255)");
			builder.Property(x => x.JailTime).HasColumnName("jailtime").HasColumnType("int(16)");
			builder.Property(x => x.Fine).HasColumnName("fine").HasColumnType("int(16)");
		}
	}
}