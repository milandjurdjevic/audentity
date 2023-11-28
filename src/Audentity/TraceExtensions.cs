using System.Collections.Immutable;

using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Audentity;

public static class TraceExtensions
{
    public static IEnumerable<Trace> ToTraces(this IEnumerable<EntityEntry> entries)
    {
        // For N:N relationships, change tracking will generate a "virtual" entry that represents the reference
        // between them, if such an entity does not exist in the source code itself.Those entries will be
        // represented as a string and object dictionary.
        return entries.Where(e => e.Metadata.ClrType != typeof(Dictionary<string, object>)).Select(Trace.Create);
    }

    public static IEnumerable<Trace> AsJoined(this IEnumerable<Trace> traces)
    {
        traces = traces.ToImmutableList();
        ImmutableList<Trace> owned = traces.Where(t => t.IsOwned).ToImmutableList();
        return traces.Where(t => !t.IsOwned).Select(t => t.Join(owned));
    }
}