using Audentity.Tests.Fixture;

using JetBrains.Annotations;

namespace Audentity.Tests;

[UsesVerify]
[TestSubject(typeof(Merge))]
public class MergeTest
{
    private readonly Database _database = new();
    private readonly Tenant _tenant = Seeding.Seed().First();

    [Fact(Skip = "Problem with trace sorting.")]
    public Task EntityAdded_Merges()
    {
        _database.Add(_tenant);
        IEnumerable<Trace> traces = _database
            .ChangeTracker
            .Entries()
            .Traces()
            .Merge();

        return VerifyJson(Serializer.Serialize(traces));
    }
}