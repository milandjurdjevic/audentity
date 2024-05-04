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
        IEnumerable<EntityTrace> traces = _database.ChangeTracker.Entries().Select(EntityTrace.FromEntry);
        IEnumerable<EntityTrace> transformed = Transform.Ownership(traces);
        return Verify(transformed);
    }
}