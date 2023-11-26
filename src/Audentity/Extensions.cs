using System.Collections.Immutable;

using Audentity.Tracking;

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Audentity;

public static class Extensions
{
    public static IReadOnlyCollection<Trace> ToTraces(this IEnumerable<EntityEntry> entries)
    {
        return entries.Select(ToTrace).ToImmutableList();
    }

    private static Property ToProperty(PropertyEntry entry)
    {
        return new Property(entry.Metadata.Name)
        {
            CurrentValue = entry.CurrentValue?.ToString(),
            OriginalValue = entry.OriginalValue?.ToString(),
            IsPrimaryKey = entry.Metadata.IsPrimaryKey(),
            IsForeignKey = entry.Metadata.IsForeignKey()
        };
    }

    private static Trace ToTrace(EntityEntry entity)
    {
        return new Trace(entity.Metadata.Name)
        {
            State = entity.State,
            IsOwned = entity.Metadata.IsOwned(),
            Properties = entity.Properties.Select(ToProperty).ToImmutableList(),
            References = entity.Navigations.SelectMany(ToReference).ToImmutableList()
        };
    }

    private static IEnumerable<Reference> ToReference(NavigationEntry navigation)
    {
        Reference result = new(navigation.Metadata.Name, navigation.Metadata.TargetEntityType.Name)
        {
            IsCollection = navigation.Metadata.IsCollection
        };

        switch (navigation)
        {
            case ReferenceEntry { TargetEntry: not null } reference:
                yield return result with
                {
                    Links = reference.TargetEntry.Properties
                        .Where(p => p.Metadata.IsPrimaryKey())
                        .Select(p => new Link(p.Metadata.Name, p.CurrentValue?.ToString() ?? String.Empty))
                        .ToImmutableList()
                };
                break;

            case CollectionEntry { CurrentValue: not null } collection:
                IEnumerable<IProperty> properties = collection.Metadata.TargetEntityType.GetProperties().ToArray();
                foreach (object entity in collection.CurrentValue)
                {
                    yield return result with
                    {
                        Links = properties.Where(p => p.IsPrimaryKey())
                            .Select(p =>
                            {
                                string? value = p.PropertyInfo?.GetValue(entity)?.ToString();
                                return new Link(p.Name, value ?? String.Empty);
                            }).ToImmutableList()
                    };
                }

                break;

            default:
                yield return result;
                break;
        }
    }
}