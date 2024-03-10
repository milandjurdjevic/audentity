using System.Collections.Immutable;

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Audentity;

public record Reference(string Name, string Target, IReadOnlyCollection<Link> Links)
{
    internal static IEnumerable<Reference> FromEntry(NavigationEntry navigation)
    {
        string name = navigation.Metadata.Name;
        string target = navigation.Metadata.TargetEntityType.Name;
        Reference result = new(name, target, ImmutableList<Link>.Empty);

        switch (navigation)
        {
            case ReferenceEntry { TargetEntry: not null } reference:
                yield return result with
                {
                    Links = reference.TargetEntry.Properties
                        .Where(p => p.Metadata.IsPrimaryKey())
                        .Select(Link.FromEntry)
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
                            .Select(p => Link.FromProperty(p, entity))
                            .ToImmutableList()
                    };
                }

                break;

            default:
                yield return result;
                break;
        }
    }
}