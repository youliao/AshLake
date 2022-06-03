using AshLake.Services.YandeStore.Infrastructure.EntityConfigurations;
using Npgsql;

namespace AshLake.Services.YandeStore.Infrastructure;

public class YandeDbContext : DbContext
{
    public DbSet<Post> Posts { get; set; } = null!;

    public YandeDbContext(DbContextOptions<YandeDbContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    static YandeDbContext()
    {
        NpgsqlConnection.GlobalTypeMapper.MapEnum<PostRating>();
        NpgsqlConnection.GlobalTypeMapper.MapEnum<PostStatus>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasPostgresEnum<PostRating>();
        builder.HasPostgresEnum<PostStatus>();

        builder.ApplyConfiguration(new PostEntityTypeConfiguration());
    }
}
