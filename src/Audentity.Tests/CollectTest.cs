using Audentity.Tests.Fixture;

using JetBrains.Annotations;

namespace Audentity.Tests;

[UsesVerify]
[TestSubject(typeof(Collect))]
public class CollectTest
{
    private readonly Database _database = new();
    private readonly Tenant _tenant = Seeding.Seed().First();

    private Task VerifyCollected()
    {
        IEnumerable<Trace> traces = _database.ChangeTracker.Entries().Collect();
        return VerifyJson(Serializer.Serialize(traces));
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
        _tenant.Name = "Updated";
        _tenant.Users.First().Name = null;
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