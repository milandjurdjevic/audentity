using System.Collections.Immutable;
using System.Collections.ObjectModel;

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Audentity;

public record ReferenceTrace
{
    private ReferenceTrace() { }

    public IReadOnlyCollection<LinkTrace> Links { get; private init; } = ReadOnlyCollection<LinkTrace>.Empty;
    public string Name { get; private init; } = String.Empty;
    public string Target { get; private init; } = String.Empty;

    internal static IEnumerable<ReferenceTrace> FromEntry(NavigationEntry navigation)
    {
        ReferenceTrace result = new()
        {
            Name = navigation.Metadata.Name, Target = navigation.Metadata.TargetEntityType.Name
        };

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