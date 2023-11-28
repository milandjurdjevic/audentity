using System.Collections.Immutable;
using System.Collections.ObjectModel;

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Audentity;

public record Reference
{
    private Reference() { }
    
    public IReadOnlyCollection<Link> Links { get; private init; } = ReadOnlyCollection<Link>.Empty;
    public string Name { get; private init; } = String.Empty;
    public string Target { get; private init; } = String.Empty;

    internal static IEnumerable<Reference> Create(NavigationEntry navigation)
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
                        .Select(p => Link.CreateLink(p))
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
                            .Select(p => Link.CreateLink(p, entity)).ToImmutableList()
                    };
                }

                break;

            default:
                yield return result;
                break;
        }
    }

    internal bool IsReferencing(Trace trace)
    {
        ImmutableList<Property> keys = trace.Properties.Where(p => p.IsPrimaryKey).ToImmutableList();
        return trace.Name == Target && Links.All(l => keys.Any(p => p.Name == l.Name && p.CurrentValue == l.Value));
    }
}