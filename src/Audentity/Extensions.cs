using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

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
        IEnumerable<EntityEntry> cache = tracker.Entries().ToArray();
        return cache.Where(IsCollectable).Select(e => e.ToEntity(cache));
    }

    private static Entity ToEntity(this EntityEntry entity, IEnumerable<EntityEntry> cache)
    {
        IEnumerable<MemberEntry> members = entity.Members.ToArray();
        IEnumerable<Property> entityProperties = members.OfType<PropertyEntry>().Select(ToProperty);
        IEnumerable<Property> referenceProperties = members.OfType<ReferenceEntry>()
            .Where(IsOwned)
            .Select(ToOwned)
            .SelectMany(cache.ToReferenceProperties);

        return new Entity
        {
            Properties = entityProperties.Concat(referenceProperties),
            State = entity.State,
            Name = entity.Metadata.Name
        };
    }

    private static IEnumerable<Property> ToReferenceProperties(this IEnumerable<EntityEntry> entries,
        (string PropertyName, EntityEntry Target) reference)
    {
        EntityEntry? old = entries.IsOldReference(reference.Target).FirstOrDefault();
        return reference.Target.Properties.Where(IsNotPrimaryKey)
            .Select(property => property.ToReferenceProperty(old));
    }

    private static Property ToReferenceProperty(this PropertyEntry entry, EntityEntry? originalReference)
    {
        PropertyEntry? originalProperty = originalReference?.Properties.Find(entry.Metadata);
        return entry.ToProperty() with { OriginalValue = originalProperty?.OriginalValue?.ToString() };
    }

    private static IEnumerable<EntityEntry> IsOldReference(this IEnumerable<EntityEntry> entries, EntityEntry target)
    {
        IOrderedEnumerable<string> keys = target.Properties.SelectKeys();
        return entries.Where(e => e.State == EntityState.Deleted &&
                                  e.Metadata.IsOwned() &&
                                  e.Metadata == target.Metadata && e.Properties.SelectKeys().SequenceEqual(keys));
    }

    private static IOrderedEnumerable<string> SelectKeys(this IEnumerable<PropertyEntry> properties)
    {
        return properties.Where(IsPrimaryKey)
            .Select(p => p.CurrentValue?.ToString() ?? String.Empty)
            .Order();
    }

    private static (string PropertyName, EntityEntry Target) ToOwned(ReferenceEntry reference)
    {
        return (reference.Metadata.Name, reference.TargetEntry!);
    }

    private static bool IsOwned(ReferenceEntry entry)
    {
        return entry.TargetEntry?.Metadata.IsOwned() == true;
    }

    private static bool IsNotPrimaryKey(PropertyEntry entry)
    {
        return !IsPrimaryKey(entry);
    }

    private static bool IsPrimaryKey(PropertyEntry entry)
    {
        return entry.Metadata.IsPrimaryKey();
    }

    private static PropertyEntry? Find(this IEnumerable<PropertyEntry> properties, IReadOnlyPropertyBase metadata)
    {
        return properties.FirstOrDefault(op => op.Metadata.Name == metadata.Name);
    }

    private static bool IsCollectable(EntityEntry entry)
    {
        bool changed = entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted;
        return !entry.Metadata.IsOwned() && (changed || entry.Navigations.Any(n => n.IsModified));
    }
}