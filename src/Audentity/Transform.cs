using System.Collections.Frozen;

namespace Audentity;

public static class Transform
{
    public static IEnumerable<Trace> Ownership(IEnumerable<Trace> entities)
    {
        entities = entities.ToArray();

        IReadOnlyCollection<Trace> owned = entities
            .Where(e => e.IsOwned)
            .ToFrozenSet();

        return entities.Where(e => !e.IsOwned).Select(e => Entity(e, owned));
    }

    private static Trace Entity(Trace trace, IReadOnlyCollection<Trace> owned)
    {
        Reference[] references = trace
            .References
            .Where(r => IsReferencing(owned, r))
            .ToArray();

        Property[] properties =
        [
            .. trace.Properties,
            .. Properties(trace.References, owned).Where(p => !p.IsPrimaryKey)
        ];
        return trace with { Properties = properties, References = references };
    }

    private static bool IsReferencing(IEnumerable<Trace> owned, Reference r)
    {
        return owned.Any(o => IsReferencing(r, o));
    }

    private static bool IsReferencing(Reference reference, Trace trace)
    {
        IReadOnlyCollection<Property> keys = trace.Properties.Where(p => p.IsPrimaryKey).ToFrozenSet();
        return trace.Type == reference.Type &&
               reference.Links.All(l => keys.Any(p => p.Name == l.Name && p.CurrentValue == l.Value));
    }

    private static IEnumerable<Property> Properties(
        IEnumerable<Reference> references,
        IReadOnlyCollection<Trace> entities
    )
    {
        foreach (Reference reference in references)
        {
            if (entities.SingleOrDefault(t => IsReferencing(reference, t)) is not { } entity)
            {
                continue;
            }

            foreach (Property property in entity.Properties)
            {
                yield return PropertyReference(property, reference);
            }

            foreach (Property property in Properties(entity.References, entities))
            {
                yield return PropertyReference(property, reference);
            }
        }
    }

    private static Property PropertyReference(Property property, Reference reference)
    {
        return property with { Name = $"{reference.Name}/{property.Name}" };
    }
}