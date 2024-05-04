using Audentity.Tests.Fixture;

namespace Audentity.Tests;

[UsesVerify]
public class CollectTest
{
    private readonly Database _database = new();
    private readonly Tenant _tenant = Seeding.Seed().First();

    private Task VerifyCollected()
    {
        return Verify(Collect.Traces(_database.ChangeTracker.Entries()));
    }

    [Fact]
    public Task EntityAdded_Collects()
    {
        _database.Add(_tenant);
        return VerifyCollected();
    }

    [Fact]
    public Task EntityModified_Collects()
    {
        _database.Add(_tenant);
        _database.SaveChanges();
        _tenant.Name += "Updated";
        _tenant.Users.First().Name += "Updated";
        _tenant.Projects.First().Name += "Updated";
        return VerifyCollected();
    }

    [Fact]
    public Task EntityDeleted_Collects()
    {
        _database.Add(_tenant);
        _database.SaveChanges();
        _database.Remove(_tenant);
        return VerifyCollected();
    }
}