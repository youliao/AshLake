using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AshLake.Services.Yande.Infrastructure.EntityConfigurations;

public class PostEntityTypeConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable(nameof(Post));
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Author)
            .HasMaxLength(50);

        builder.Property(x => x.FileExt)
            .IsRequired()
            .HasMaxLength(4);

        builder.Property(x => x.Md5)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(32);

        builder.Property(x => x.Rating)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        builder.Property(x => x.Tags)
            .HasColumnType("character varying[]");

        builder.HasIndex(x => x.Tags)
            .HasMethod("gin");

        builder.HasIndex(x => x.Score);
    }
}
