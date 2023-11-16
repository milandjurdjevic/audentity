namespace Audentity.Tests;

[UsesVerify]
public class AuditTests
{
    private readonly Database _database = new();

    [Fact]
    public Task Added_Collects()
    {
        _database.Add(new Model());
        return Verify(_database.ChangeTracker.Audit());
    }

    [Fact]
    public Task Modified_Collects()
    {
        Model entity = new();
        _database.Add(entity);
        _database.SaveChanges();
        entity.Property = Guid.NewGuid().ToString();
        entity.Owned.Value = Guid.NewGuid().ToString();
        entity.Owned.Immutable = entity.Owned.Immutable with { Value = Guid.NewGuid().ToString() };
        return Verify(_database.ChangeTracker.Audit());
    }

    [Fact]
    public Task Deleted_Collects()
    {
        Model entity = new();
        _database.Add(entity);
        _database.SaveChanges();
        _database.Remove(entity);
        return Verify(_database.ChangeTracker.Audit());
    }
}