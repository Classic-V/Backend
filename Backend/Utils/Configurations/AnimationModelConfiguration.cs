using Backend.Utils.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Utils.Configurations;

public class AnimationModelConfiguration : IEntityTypeConfiguration<AnimationModel>
{
    public void Configure(EntityTypeBuilder<AnimationModel> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("server_animations");
        builder.HasIndex(x => x.Id).HasDatabaseName("id");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int(11)");
        builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)");
        builder.Property(x => x.Category).HasColumnName("category").HasColumnType("int(11)");
        builder.Property(x => x.AnimationDictionary).HasColumnName("anim_dict").HasColumnType("varchar(255)");
        builder.Property(x => x.AnimationName).HasColumnName("anim_name").HasColumnType("varchar(255)");
        builder.Property(x => x.AnimationFlag).HasColumnName("anim_flag").HasColumnType("int(11)");
    }
}