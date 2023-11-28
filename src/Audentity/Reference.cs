using System.Collections.Immutable;
using System.Collections.ObjectModel;

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Audentity;

public record Reference
{
    public IReadOnlyCollection<Link> Links { get; init; } = ReadOnlyCollection<Link>.Empty;
    public string Name { get; init; } = String.Empty;
    public string Target { get; init; } = String.Empty;

    public static IEnumerable<Reference> Create(NavigationEntry navigation)
    {
        Reference result = new()
        {
            Name = navigation.Metadata.Name,
            Target = navigation.Metadata.TargetEntityType.Name
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

    public bool IsReferencing(Trace trace)
    {
        ImmutableList<Property> keys = trace.Properties.Where(p => p.IsPrimaryKey).ToImmutableList();
        return trace.Name == Target && Links.All(l => keys.Any(p => p.Name == l.Name && p.CurrentValue == l.Value));
    }
}