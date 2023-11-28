using System.Collections.Immutable;

namespace Audentity;

public static class TraceExtensions
{
    public static IEnumerable<Trace> AsJoined(this IEnumerable<Trace> traces)
    {
        traces = traces.ToImmutableList();
        ImmutableList<Trace> owned = traces.Where(t => t.IsOwned).ToImmutableList();
        return traces.Where(t => !t.IsOwned).Select(t => t.Join(owned));
    }
}