namespace Audentity.Tests;

public class Database : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<RootEntity>()
            .OwnsOne(e => e.Owned, b => b.OwnsOne(o => o.Nested));
    }
}