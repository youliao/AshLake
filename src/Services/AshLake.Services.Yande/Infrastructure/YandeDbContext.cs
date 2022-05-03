using AshLake.Services.Yande.Infrastructure.EntityConfigurations;

namespace AshLake.Services.Yande.Infrastructure;

public class YandeDbContext : DbContext
{
    public DbSet<Post> Posts { get; set; }

    public YandeDbContext(DbContextOptions<YandeDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasPostgresEnum<PostStatus>();

        builder.ApplyConfiguration(new PostEntityTypeConfiguration());
    }
}
