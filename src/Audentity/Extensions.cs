using System.Collections.ObjectModel;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Audentity;

public static class Extensions
{
    public static IEnumerable<EntityLog> Audit(this ChangeTracker tracker)
    {
        EntityEntry[] pool = tracker.Entries().ToArray();
        return pool.Where(e => !IsOwned(e) && IsChanged(e))
            .Select(e => e.ToLog(pool));
    }

    private static PropertyLog ToLog(this PropertyEntry entry)
    {
        string currentValue = entry.CurrentValue?.ToString() ?? String.Empty;
        string originalValue = entry.OriginalValue?.ToString() ?? String.Empty;
        PropertyValue propertyValue = !entry.IsModified
            ? new PropertyValue(currentValue)
            : new ModifiedPropertyValue(currentValue, originalValue);

        return new PropertyLog
        {
            IsPrimaryKey = entry.Metadata.IsPrimaryKey(),
            Value = propertyValue,
            Name = entry.Metadata.Name,
            Owner = entry.Metadata.DeclaringEntityType.Name
        };
    }

    private static EntityLog ToLog(this EntityEntry entry, EntityEntry[] pool)
    {
        IEnumerable<PropertyLog> properties = entry.Properties.Select(ToLog)
            .Concat(GetReferenceProperties(entry, pool));

        return new EntityLog { Properties = properties, State = entry.State, Name = entry.Metadata.Name };
    }

    private static IEnumerable<PropertyLog> GetReferenceProperties(EntityEntry entity, EntityEntry[] pool,
        string path = "")
    {
        foreach (NavigationEntry navigation in entity.Navigations.Where(n => n.Metadata.TargetEntityType.IsOwned()))
        {
            if (navigation is not ReferenceEntry { TargetEntry: not null } reference)
            {
                continue;
            }

            ReadOnlyDictionary<string, object?> deletedValuesCache = reference.TargetEntry.GetClones(pool)
                .FirstOrDefault(e => InDifferentState(entity, e) && IsDeleted(e) && IsOwned(e))
                .CacheCurrentPropertyValues();

            string referencePath = $"{path}:{navigation.Metadata.Name}".TrimStart(':');

            IEnumerable<PropertyLog> properties = reference.TargetEntry
                .Properties
                .Where(IsNotPrimaryKey)
                .Select(p => p.ToReferenceProperty(deletedValuesCache, referencePath))
                .Concat(GetReferenceProperties(reference.TargetEntry, pool, referencePath));

            foreach (PropertyLog property in properties)
            {
                yield return property;
            }
        }
    }

    private static bool InDifferentState(EntityEntry left, EntityEntry right)
    {
        return right.State != left.State;
    }

    private static ReadOnlyDictionary<string, object?> CacheCurrentPropertyValues(this EntityEntry? entry)
    {
        return entry is null
            ? new Dictionary<string, object?>().AsReadOnly()
            : entry.Properties.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue).AsReadOnly();
    }

    private static bool IsOwned(EntityEntry entity)
    {
        return entity.Metadata.IsOwned();
    }

    private static bool IsDeleted(EntityEntry entity)
    {
        return entity.State is EntityState.Deleted;
    }

    private static PropertyLog ToReferenceProperty(this PropertyEntry entry,
        IReadOnlyDictionary<string, object?> originalValues, string referencePath)
    {
        PropertyLog property = entry.ToLog() with { Name = $"{referencePath}:{entry.Metadata.Name}" };

        if (!originalValues.ContainsKey(entry.Metadata.Name))
        {
            return property;
        }

        string original = originalValues[entry.Metadata.Name]?.ToString() ?? String.Empty;
        ModifiedPropertyValue value = new(property.Value.Current, original);
        return property with { Value = value };
    }

    private static IEnumerable<EntityEntry> GetClones(this EntityEntry entity, IEnumerable<EntityEntry> pool)
    {
        IOrderedEnumerable<string> keys = entity.Properties.SelectKeys();
        return pool.Where(e => e.Metadata == entity.Metadata && e.Properties.SelectKeys().SequenceEqual(keys));
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

    private static bool IsChanged(EntityEntry entry)
    {
        bool changed = entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted;
        return changed || entry.Navigations.Any(n => n.IsModified);
    }
}