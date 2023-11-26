using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Audentity.Tracking;

public static class Extensions
{
    public static IEnumerable<Trace> Track(this ChangeTracker tracker)
    {
        return tracker.Entries().Select(ToTrace);
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
            Properties = entity.Properties.Select(ToProperty),
            References = entity.Navigations.SelectMany(ToReference)
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
                            })
                    };
                }

                break;

            default:
                yield return result;
                break;
        }
    }
}