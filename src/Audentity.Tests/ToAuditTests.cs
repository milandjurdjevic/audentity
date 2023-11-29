namespace Audentity.Tests;

[UsesVerify]
public class ToAuditTests
{
    private readonly Database _database = new();
    private readonly Tenant _tenant = Seeding.Seed().First();

    [Fact]
    public Task AddEntity_Collects()
    {
        _database.Add(_tenant);
        return Verify(_database.ChangeTracker.Entries().ToAudit());
    }

    [Fact]
    public Task ModifyEntity_Collects()
    {
        _database.Add(_tenant);
        _database.SaveChanges();
        _tenant.Name += "Updated";
        _tenant.Users.First().Name += "Updated";
        _tenant.Projects.First().Name += "Updated";
        IEnumerable<EntityTrace> traces = _database.ChangeTracker
            .Entries()
            .ToAudit();
        return Verify(traces);
    }

    [Fact]
    public Task DeleteEntity_Collects()
    {
        _database.Add(_tenant);
        _database.SaveChanges();
        _database.Remove(_tenant);
        IEnumerable<EntityTrace> traces = _database.ChangeTracker
            .Entries()
            .ToAudit();
        return Verify(traces);
    }
}