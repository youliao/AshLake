using AshLake.Services.Yande.Infrastructure.EntityConfigurations;
using Npgsql;

namespace AshLake.Services.Yande.Infrastructure;

public class YandeDbContext : DbContext
{
    public DbSet<Post> Posts { get; set; }

    public YandeDbContext(DbContextOptions<YandeDbContext> options) : base(options)
    {
    }

    static YandeDbContext()
    {
        NpgsqlConnection.GlobalTypeMapper.MapEnum<PostStatus>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasPostgresEnum<PostStatus>();
        builder.ApplyConfiguration(new PostEntityTypeConfiguration());
    }
}
