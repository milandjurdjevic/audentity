using Audentity.Tests.Fixture;

namespace Audentity.Tests;

[UsesVerify]
public class TransformTest
{
    private readonly Database _database = new();
    private readonly Tenant _tenant = Seeding.Seed().First();

    [Fact]
    public Task Ownership()
    {
        _database.Add(_tenant);
        IEnumerable<Trace> traces = Collect.Traces(_database.ChangeTracker.Entries());
        IEnumerable<Trace> transformed = Transform.Ownership(traces);
        return Verify(transformed);
    }
}