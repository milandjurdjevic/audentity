using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Audentity;

public record EntityTrace(
    Type Type,
    EntityState State,
    IReadOnlyCollection<PropertyTrace> Properties,
    IReadOnlyCollection<ReferenceTrace> References,
    bool IsOwned)
{
    public static EntityTrace FromEntry(EntityEntry entry)
    {
        IReadOnlyCollection<PropertyTrace> properties =
            entry
                .Properties
                .Select(PropertyTrace.FromEntry)
                .ToArray();

        IReadOnlyCollection<ReferenceTrace> references =
            entry
                .Navigations
                .SelectMany(ReferenceTrace.FromEntry)
                .ToArray();

        return new EntityTrace(entry.Entity.GetType(), entry.State, properties, references, entry.Metadata.IsOwned());
    }
}