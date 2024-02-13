using System.Collections.Immutable;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Audentity;

public record EntityTrace(
    string Name,
    EntityState State,
    IReadOnlyCollection<PropertyTrace> Properties,
    IReadOnlyCollection<ReferenceTrace> References)
{
    public static EntityTrace FromEntry(EntityEntry entry)
    {
        ImmutableList<PropertyTrace> properties =
            entry
                .Properties
                .Select(PropertyTrace.FromEntry)
                .ToImmutableList();

        ImmutableList<ReferenceTrace> references =
            entry
                .Navigations
                .SelectMany(ReferenceTrace.FromEntry)
                .ToImmutableList();

        return new EntityTrace(entry.Metadata.Name, entry.State, properties, references);
    }
}