using System.Collections.Immutable;
using System.Collections.ObjectModel;

using Audentity.Tracking;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Audentity;

public record Trace(string Name)
{
    public IReadOnlyCollection<Property> Properties { get; init; } = ReadOnlyCollection<Property>.Empty;
    public IReadOnlyCollection<Reference> References { get; init; } = ReadOnlyCollection<Reference>.Empty;
    public EntityState State { get; init; } = EntityState.Unchanged;
    public bool IsOwned { get; init; }

    public static Trace Create(EntityEntry entity)
    {
        return new Trace(entity.Metadata.Name)
        {
            State = entity.State,
            IsOwned = entity.Metadata.IsOwned(),
            Properties = entity.Properties.Select(Property.Create).ToImmutableList(),
            References = entity.Navigations.SelectMany(Reference.Create).ToImmutableList()
        };
    }
}