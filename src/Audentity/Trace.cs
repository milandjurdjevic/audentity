using System.Collections.Immutable;
using System.Collections.ObjectModel;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Audentity;

public record Trace
{
    private Trace() { }

    public string Name { get; private init; } = String.Empty;
    public IReadOnlyCollection<Property> Properties { get; private init; } = ReadOnlyCollection<Property>.Empty;
    public IReadOnlyCollection<Reference> References { get; private init; } = ReadOnlyCollection<Reference>.Empty;
    public EntityState State { get; private init; } = EntityState.Unchanged;
    public bool IsOwned { get; private init; }

    internal static Trace Create(EntityEntry entity)
    {
        return new Trace
        {
            Name = entity.Metadata.Name,
            State = entity.State,
            IsOwned = entity.Metadata.IsOwned(),
            Properties = entity.Properties.Select(Property.Create).ToImmutableList(),
            References = entity.Navigations.SelectMany(Reference.Create).ToImmutableList()
        };
    }
}