using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Audentity;

public class Auditable
{
    private Auditable(EntityEntry entry, IReadOnlyCollection<Property> properties)
    {
        Properties = properties;
        State = entry.State;
        Name = entry.Metadata.Name;
    }

    public IReadOnlyCollection<Property> Properties { get; }
    public EntityState State { get; }
    public string Name { get; }

    private static Auditable Collect(EntityEntry entry)
    {
        return new Auditable(entry, Property.Collect(entry));
    }

    public static IEnumerable<Auditable> Collect(ChangeTracker tracker)
    {
        return tracker.Entries()
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(Collect);
    }
}