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

    public static IEnumerable<Auditable> Collect(ChangeTracker tracker)
    {
        IEnumerable<EntityEntry> entries = tracker.Entries().ToArray();

        return entries.Where(e => !e.Metadata.IsOwned() &&
                                  (e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted ||
                                   e.Navigations.Any(n => n.IsModified)))
            .Select(e =>
            {
                IEnumerable<MemberEntry> members = e.Members.ToArray();

                IEnumerable<Property> properties = members.OfType<PropertyEntry>()
                    .Select(p => new Property(p));

                IEnumerable<Property> references = members.OfType<ReferenceEntry>()
                    .Where(r => r.Metadata.TargetEntityType.IsOwned())
                    .SelectMany(r =>
                    {
                        IEnumerable<string> keys = r.TargetEntry?.Properties
                            .Where(p => p.Metadata.IsPrimaryKey())
                            .Select(p => p.CurrentValue?.ToString() ?? String.Empty)
                            .Order() ?? Enumerable.Empty<string>();

                        // Find original reference.
                        EntityEntry? original = entries.FirstOrDefault(or =>
                            or.State == EntityState.Deleted &&
                            or.Metadata.IsOwned() &&
                            or.Metadata == r.Metadata.TargetEntityType && or
                                .Properties
                                .Where(orp => orp.Metadata.IsPrimaryKey())
                                .Select(orp =>
                                    orp.CurrentValue?.ToString() ??
                                    String.Empty)
                                .Order().SequenceEqual(keys));


                        return r.TargetEntry!.Properties.Where(xp => !xp.Metadata.IsPrimaryKey()).Select(rp =>
                        {
                            ReferenceProperty property = new(rp);
                            return original?.Properties.FirstOrDefault(op => op.Metadata.Name == rp.Metadata.Name) is
                                { } propertyEntry
                                ? property with { OriginalValue = propertyEntry.OriginalValue?.ToString() }
                                : property;
                        });
                    });


                return new Auditable(e, properties.Concat(references).ToArray());
            });
    }
}