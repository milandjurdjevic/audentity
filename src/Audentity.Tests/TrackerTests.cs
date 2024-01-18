namespace Audentity.Tests;

[UsesVerify]
public class TrackerTests
{
    private readonly Database _database = new();
    private readonly Tenant _tenant = Seeding.Seed().First();

    [Fact]
    public Task AddEntity_Collects()
    {
        _database.Add(_tenant);
        IEnumerable<Entity> traces = Tracker.OfEntries(_database.ChangeTracker.Entries());
        return Verify(traces);
    }

    [Fact]
    public Task ModifyEntity_Collects()
    {
        _database.Add(_tenant);
        _database.SaveChanges();
        _tenant.Name += "Updated";
        _tenant.Users.First().Name += "Updated";
        _tenant.Projects.First().Name += "Updated";
        IEnumerable<Entity> traces = Tracker.OfEntries(_database.ChangeTracker.Entries());
        return Verify(traces);
    }

    [Fact]
    public Task DeleteEntity_Collects()
    {
        _database.Add(_tenant);
        _database.SaveChanges();
        _database.Remove(_tenant);
        IEnumerable<Entity> traces = Tracker.OfEntries(_database.ChangeTracker.Entries());
        return Verify(traces);
    }
}