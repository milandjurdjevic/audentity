namespace Audentity.Tests;

[UsesVerify]
public class TraceLinkTests
{
    private readonly Database _database = new();

    [Fact]
    public Task Link_LinksOwnedTraceProperties()
    {
        _database.AddRange(Seeding.Seed());
        IEnumerable<Trace> traces = _database.ChangeTracker.Entries()
            .Select(Trace.Create)
            .AsJoined();

        return Verify(traces);
    }
}