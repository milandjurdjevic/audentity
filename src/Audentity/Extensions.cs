using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Audentity;

public static class Extensions
{
    private static Property ToProperty(this PropertyEntry entry)
    {
        return new Property
        {
            IsPrimaryKey = entry.Metadata.IsPrimaryKey(),
            OriginalValue = entry.EntityEntry.State == EntityState.Added ? null : entry.OriginalValue?.ToString(),
            CurrentValue = entry.CurrentValue?.ToString(),
            Name = entry.Metadata.Name,
            Owner = entry.Metadata.DeclaringEntityType.Name
        };
    }

    public static IEnumerable<Entity> Audit(this ChangeTracker tracker)
    {
        EntityEntry[] entries = tracker.Entries().ToArray();
        return entries.Where(HasChanges).Select(e => e.ToEntity(entries));
    }

    private static Entity ToEntity(this EntityEntry entry, EntityEntry[] entries)
    {
        IEnumerable<Property> properties = entry.Properties.Select(ToProperty)
            .Concat(GetReferenceProperties(entry, entries));

        return new Entity { Properties = properties, State = entry.State, Name = entry.Metadata.Name };
    }

    private static IEnumerable<Property> GetReferenceProperties(EntityEntry entity, EntityEntry[] entities,
        string path = "")
    {
        foreach (NavigationEntry navigation in entity.Navigations.Where(n => n.Metadata.TargetEntityType.IsOwned()))
        {
            if (navigation is not ReferenceEntry { TargetEntry: not null } reference)
            {
                continue;
            }

            Dictionary<string, string?>? originalValues = reference.TargetEntry
                .GetDeletedReferences(entities)
                .FirstOrDefault()?
                .Properties
                .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue?.ToString());
            string referencePath = $"{path}:{navigation.Metadata.Name}".TrimStart(':');
            IEnumerable<Property> properties = reference.TargetEntry
                .Properties
                .Where(IsNotPrimaryKey)
                .Select(p => p.ToReferenceProperty(referencePath, originalValues))
                .Concat(GetReferenceProperties(reference.TargetEntry, entities, referencePath));

            foreach (Property property in properties)
            {
                yield return property;
            }
        }
    }

    private static Property ToReferenceProperty(this PropertyEntry entry, string referencePath,
        IReadOnlyDictionary<string, string?>? originalValues)
    {
        return entry.ToProperty() with
        {
            Name = $"{referencePath}:{entry.Metadata.Name}",
            OriginalValue = originalValues?.GetValueOrDefault(entry.Metadata.Name)
        };
    }

    private static IEnumerable<EntityEntry> GetDeletedReferences(this EntityEntry entry,
        IEnumerable<EntityEntry> entries)
    {
        IOrderedEnumerable<string> keys = entry.Properties.SelectKeys();
        return entries.Where(e => e.State == EntityState.Deleted &&
                                  e.Metadata.IsOwned() &&
                                  e.Metadata == entry.Metadata &&
                                  e.Properties.SelectKeys().SequenceEqual(keys));
    }

    private static IOrderedEnumerable<string> SelectKeys(this IEnumerable<PropertyEntry> properties)
    {
        return properties.Where(IsPrimaryKey)
            .Select(p => p.CurrentValue?.ToString() ?? String.Empty)
            .Order();
    }

    private static bool IsPrimaryKey(PropertyEntry entry)
    {
        return entry.Metadata.IsPrimaryKey();
    }

    private static bool IsNotPrimaryKey(PropertyEntry entry)
    {
        return !IsPrimaryKey(entry);
    }

    private static bool HasChanges(EntityEntry entry)
    {
        bool changed = entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted;
        return !entry.Metadata.IsOwned() && (changed || entry.Navigations.Any(n => n.IsModified));
    }
}