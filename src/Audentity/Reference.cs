using System.Collections.ObjectModel;

namespace Audentity.Tracking;

public record Reference(string Name, string Target)
{
    public IReadOnlyCollection<Link> Links { get; init; } = ReadOnlyCollection<Link>.Empty;
    public bool IsCollection { get; init; }
}