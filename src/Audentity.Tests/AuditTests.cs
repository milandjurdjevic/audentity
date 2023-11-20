namespace Audentity.Tests;

[UsesVerify]
public class AuditTests
{
    private readonly Database _database = new();

    [Fact]
    public Task ReferenceRemoved_Collects()
    {
        Model model = new();
        _database.Add(model);
        _database.SaveChanges();
        model.Owned = null;
        return Verify(_database.ChangeTracker.Audit());
    }

    [Fact]
    public Task NestedReferenceRemoved_Collects()
    {
        Model model = new();
        _database.Add(model);
        _database.SaveChanges();
        model.Owned!.Immutable = null;
        return Verify(_database.ChangeTracker.Audit());
    }

    [Fact]
    public Task Added_Collects()
    {
        _database.Add(new Model());
        return Verify(_database.ChangeTracker.Audit());
    }

    [Fact]
    public Task RangeAdded_Collects()
    {
        Model[] entities = { new(), new() };
        _database.Set<Model>().AddRange(entities);
        return Verify(_database.ChangeTracker.Audit());
    }

    [Fact]
    public Task Modified_Collects()
    {
        Model entity = new();
        _database.Add(entity);
        _database.SaveChanges();
        entity.Property = Guid.NewGuid().ToString();
        entity.Owned!.Value = Guid.NewGuid().ToString();
        entity.Owned.Immutable = entity.Owned.Immutable! with { Value = Guid.NewGuid().ToString() };
        return Verify(_database.ChangeTracker.Audit());
    }

    [Fact]
    public Task RangeModified_Collects()
    {
        Model[] entities = { new(), new() };
        _database.Set<Model>().AddRange(entities);
        _database.SaveChanges();

        foreach (Model entity in entities)
        {
            entity.Property = Guid.NewGuid().ToString();
            entity.Owned!.Value = Guid.NewGuid().ToString();
            entity.Owned.Immutable = entity.Owned.Immutable! with { Value = Guid.NewGuid().ToString() };
        }

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

    [Fact]
    public Task RangeDeleted_Collects()
    {
        Model[] entities = { new(), new() };
        _database.Set<Model>().AddRange(entities);
        _database.SaveChanges();
        _database.Set<Model>().RemoveRange(entities);
        return Verify(_database.ChangeTracker.Audit());
    }
}