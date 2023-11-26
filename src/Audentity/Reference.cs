using System.Collections.Immutable;
using System.Collections.ObjectModel;

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Audentity.Tracking;

public record Reference(string Name, string Target)
{
    public IReadOnlyCollection<Link> Links { get; init; } = ReadOnlyCollection<Link>.Empty;
    public bool IsCollection { get; init; }

    public static IEnumerable<Reference> Create(NavigationEntry navigation)
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