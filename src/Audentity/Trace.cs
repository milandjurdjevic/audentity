using System.Collections.ObjectModel;

using Audentity.Tracking;

using Microsoft.EntityFrameworkCore;

namespace Audentity;

public record Trace(string Name)
{
    public IReadOnlyCollection<Property> Properties { get; init; } = ReadOnlyCollection<Property>.Empty;
    public IReadOnlyCollection<Reference> References { get; init; } = ReadOnlyCollection<Reference>.Empty;
    public EntityState State { get; init; } = EntityState.Unchanged;
    public bool IsOwned { get; init; }
}