using System.Collections.Frozen;

namespace Audentity;

public static class Transform
{
    public static IEnumerable<EntityTrace> Ownership(IEnumerable<EntityTrace> entities)
    {
        entities = entities.ToArray();

        IReadOnlyCollection<EntityTrace> owned = entities
            .Where(e => e.IsOwned)
            .ToFrozenSet();

        return entities.Where(e => !e.IsOwned).Select(e => Entity(e, owned));
    }

    private static EntityTrace Entity(EntityTrace entity, IReadOnlyCollection<EntityTrace> owned)
    {
        ReferenceTrace[] references = entity
            .References
            .Where(r => ReferencesAny(owned, r))
            .ToArray();

        PropertyTrace[] properties =
        [
            .. entity.Properties,
            .. Properties(entity.References, owned).Where(p => !p.IsPrimaryKey)
        ];
        return entity with { Properties = properties, References = references };
    }

    private static bool ReferencesAny(IEnumerable<EntityTrace> owned, ReferenceTrace r)
    {
        return owned.Any(o => References(r, o));
    }

    private static bool References(ReferenceTrace reference, EntityTrace entity)
    {
        IReadOnlyCollection<PropertyTrace> keys = entity.Properties.Where(p => p.IsPrimaryKey).ToFrozenSet();
        return entity.Type == reference.Type &&
               reference.Links.All(l => keys.Any(p => p.Name == l.Name && p.CurrentValue == l.Value));
    }
    
    private static IEnumerable<PropertyTrace> Properties(
        IEnumerable<ReferenceTrace> references,
        IReadOnlyCollection<EntityTrace> entities
    )
    {
        foreach (ReferenceTrace reference in references)
        {
            if (entities.SingleOrDefault(t => References(reference, t)) is not { } entity)
            {
                continue;
            }

            foreach (PropertyTrace property in entity.Properties)
            {
                yield return PropertyReference(property, reference);
            }

            foreach (PropertyTrace property in Properties(entity.References, entities))
            {
                yield return PropertyReference(property, reference);
            }
        }
    }


    private static PropertyTrace PropertyReference(PropertyTrace property, ReferenceTrace reference)
    {
        return property with { Name = $"{reference.Name}/{property.Name}" };
    }
}