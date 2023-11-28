namespace Audentity.Tests;

[UsesVerify]
public class TraceJoinTests
{
    private readonly Database _database = new();

    [Fact]
    public Task AsJoined_JoinsOwnedProperties()
    {
        _database.AddRange(Seeding.Seed());
        IEnumerable<Trace> traces = _database.ChangeTracker.Entries()
            .ToTrace()
            .AsJoined();

        return Verify(traces);
    }
}