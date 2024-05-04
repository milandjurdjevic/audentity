using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Audentity;

public static class Collect
{
    public static IEnumerable<Trace> Traces(IEnumerable<EntityEntry> entries)
    {
        return entries.Select(Trace);
    }

    private static Trace Trace(EntityEntry entry)
    {
        IReadOnlyCollection<Property> properties =
            entry
                .Properties
                .Select(Property)
                .ToArray();

        IReadOnlyCollection<Reference> references =
            entry
                .Navigations
                .SelectMany(References)
                .ToArray();

        return new Trace(entry.Entity.GetType(), entry.State, properties, references, entry.Metadata.IsOwned());
    }

    private static Link Link(IReadOnlyPropertyBase property, object entity)
    {
        string value = property.PropertyInfo?.GetValue(entity)?.ToString() ?? String.Empty;
        return new Link(property.Name, value);
    }

    private static Link Link(PropertyEntry entry)
    {
        string value = entry.CurrentValue?.ToString() ?? String.Empty;
        return new Link(entry.Metadata.Name, value);
    }

    private static Property Property(PropertyEntry entry)
    {
        string? currentValue = entry.CurrentValue?.ToString();
        string? originalValue = entry.OriginalValue?.ToString();
        string name = entry.Metadata.Name;
        return new Property(name, currentValue, originalValue, entry.Metadata.IsPrimaryKey());
    }

    private static IEnumerable<Reference> References(NavigationEntry navigation)
    {
        string name = navigation.Metadata.Name;
        Type type = navigation.Metadata.TargetEntityType.ClrType;

        switch (navigation)
        {
            case ReferenceEntry { TargetEntry: not null } reference:
                {
                    IReadOnlyCollection<Link> links =
                        reference
                            .TargetEntry
                            .Properties
                            .Where(p => p.Metadata.IsPrimaryKey())
                            .Select(Link)
                            .ToArray();

                    yield return new Reference(name, type, links);
                    break;
                }

            case CollectionEntry { CurrentValue: not null } collection:
                {
                    IReadOnlyCollection<IProperty> primaryKeys =
                        collection
                            .Metadata
                            .TargetEntityType
                            .GetProperties()
                            .Where(p => p.IsPrimaryKey())
                            .ToArray();

                    foreach (object entity in collection.CurrentValue)
                    {
                        IReadOnlyCollection<Link> links =
                            primaryKeys
                                .Select(p => Link(p, entity))
                                .ToArray();

                        yield return new Reference(name, type, links);
                    }

                    break;
                }

            default:
                yield return new Reference(name, type, []);
                break;
        }
    }
}