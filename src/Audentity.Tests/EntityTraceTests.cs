namespace Audentity.Tests;

[UsesVerify]
public class EntityTraceTests
{
    private readonly Database _database = new();
    private readonly Tenant _tenant = Seeding.Seed().First();

    [Fact]
    public Task FromEntry_AddEntity_Collects()
    {
        _database.Add(_tenant);
        IEnumerable<EntityTrace> traces = _database
            .ChangeTracker
            .Entries()
            .Select(EntityTrace.FromEntry);

        return Verify(traces);
    }

    [Fact]
    public Task FromEntry_ModifyEntity_Collects()
    {
        _database.Add(_tenant);
        _database.SaveChanges();
        _tenant.Name += "Updated";
        _tenant.Users.First().Name += "Updated";
        _tenant.Projects.First().Name += "Updated";
        IEnumerable<EntityTrace> traces = _database
            .ChangeTracker
            .Entries()
            .Select(EntityTrace.FromEntry);
        return Verify(traces);
    }

    [Fact]
    public Task FromEntry_DeleteEntity_Collects()
    {
        _database.Add(_tenant);
        _database.SaveChanges();
        _database.Remove(_tenant);
        IEnumerable<EntityTrace> traces = _database.ChangeTracker
            .Entries()
            .Select(EntityTrace.FromEntry);
        return Verify(traces);
    }
}