using System.Collections.Immutable;

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Audentity;

public record ReferenceTrace(string Name, string Target, IReadOnlyCollection<LinkTrace> Links)
{
    internal static IEnumerable<ReferenceTrace> FromEntry(NavigationEntry navigation)
    {
        string name = navigation.Metadata.Name;
        string target = navigation.Metadata.TargetEntityType.Name;
        ReferenceTrace result = new(name, target, ImmutableList<LinkTrace>.Empty);

        switch (navigation)
        {
            case ReferenceEntry { TargetEntry: not null } reference:
                yield return result with
                {
                    Links = reference.TargetEntry.Properties
                        .Where(p => p.Metadata.IsPrimaryKey())
                        .Select(LinkTrace.FromEntry)
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
                            .Select(p => LinkTrace.FromProperty(p, entity))
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