using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Audentity;

public record ReferenceTrace(string Name, Type Type, IReadOnlyCollection<LinkTrace> Links)
{
    internal static IEnumerable<ReferenceTrace> FromEntry(NavigationEntry navigation)
    {
        string name = navigation.Metadata.Name;
        Type type = navigation.Metadata.TargetEntityType.ClrType;

        switch (navigation)
        {
            case ReferenceEntry { TargetEntry: not null } reference:
                {
                    IReadOnlyCollection<LinkTrace> links = reference
                        .TargetEntry
                        .Properties
                        .Where(p => p.Metadata.IsPrimaryKey())
                        .Select(LinkTrace.FromEntry)
                        .ToArray();

                    yield return new ReferenceTrace(name, type, links);
                    break;
                }

            case CollectionEntry { CurrentValue: not null } collection:
                {
                    IReadOnlyCollection<IProperty> primaryKeys = collection
                        .Metadata
                        .TargetEntityType
                        .GetProperties()
                        .Where(p => p.IsPrimaryKey())
                        .ToArray();

                    foreach (object entity in collection.CurrentValue)
                    {
                        IReadOnlyCollection<LinkTrace> links = primaryKeys
                            .Select(p => LinkTrace.FromProperty(p, entity))
                            .ToArray();

                        yield return new ReferenceTrace(name, type, links);
                    }

                    break;
                }

            default:
                yield return new ReferenceTrace(name, type, []);
                break;
        }
    }
}