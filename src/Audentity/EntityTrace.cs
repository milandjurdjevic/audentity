using System.Collections.Immutable;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Audentity;

public record EntityTrace
{
    private EntityTrace() { }
    public string Name { get; private init; } = String.Empty;
    public IReadOnlyCollection<PropertyTrace> Properties { get; private init; } = ImmutableList<PropertyTrace>.Empty;
    public IReadOnlyCollection<ReferenceTrace> References { get; private init; } = ImmutableList<ReferenceTrace>.Empty;
    public EntityState State { get; private init; } = EntityState.Unchanged;
    public bool IsOwned { get; private init; }

    internal static EntityTrace Create(EntityEntry entry)
    {
        return new EntityTrace
        {
            Name = entry.Metadata.Name,
            State = entry.State,
            IsOwned = entry.Metadata.IsOwned(),
            Properties = entry.Properties.Select(PropertyTrace.Create).ToImmutableList(),
            References = entry.Navigations.SelectMany(ReferenceTrace.Create).ToImmutableList()
        };
    }
}