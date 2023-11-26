using System.Collections.Immutable;

namespace Audentity.Tests;

[UsesVerify]
public class TraceExtensionsTests
{
    private readonly Database _database = new();

    [Fact]
    public Task AggregateOwned_AggregatesOwnedTraceProperties()
    {
        _database.AddRange(Seeding.Seed());
        IEnumerable<Trace> traces = _database.ChangeTracker
            .Entries()
            .ToTraces()
            .AggregateOwned()
            .ToImmutableList();

        return Verify(traces);
    }
}