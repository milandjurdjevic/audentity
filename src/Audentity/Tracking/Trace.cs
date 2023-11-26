using Microsoft.EntityFrameworkCore;

namespace Audentity.Tracking;

public record Trace(string Name)
{
    public IEnumerable<Property> Properties { get; init; } = Enumerable.Empty<Property>();
    public IEnumerable<Reference> References { get; init; } = Enumerable.Empty<Reference>();
    public EntityState State { get; internal init; } = EntityState.Unchanged;
    public bool IsOwned { get; init; }
}