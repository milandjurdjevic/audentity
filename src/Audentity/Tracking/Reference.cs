namespace Audentity.Tracking;

public record Reference(string Name, string Target)
{
    public IEnumerable<Link> Links { get; init; } = Enumerable.Empty<Link>();
    public bool IsCollection { get; init; }
}