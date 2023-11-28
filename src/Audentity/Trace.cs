using System.Collections.Immutable;
using System.Collections.ObjectModel;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Audentity;

public record Trace
{
    public string Name { get; private init; } = String.Empty;
    public IReadOnlyCollection<Property> Properties { get; init; } = ReadOnlyCollection<Property>.Empty;
    public IReadOnlyCollection<Reference> References { get; init; } = ReadOnlyCollection<Reference>.Empty;
    public EntityState State { get; init; } = EntityState.Unchanged;
    public bool IsOwned { get; init; }
    public bool IsVirtual { get; init; }

    public static Trace Create(EntityEntry entity)
    {
        return new Trace
        {
            Name = entity.Metadata.Name,
            State = entity.State,
            IsOwned = entity.Metadata.IsOwned(),
            Properties = entity.Properties.Select(Property.Create).ToImmutableList(),
            References = entity.Navigations.SelectMany(Reference.Create).ToImmutableList(),
            // For N:N relationships, change tracking will generate a "virtual" entry that represents the reference
            // between them, if such an entity does not exist in the source code itself.Those entries will be
            // represented as a string and object dictionary. 
            IsVirtual = entity.Metadata.ClrType == typeof(Dictionary<string, object>)
        };
    }

    public Trace Join(ImmutableList<Trace> traces)
    {
        IEnumerable<Reference> references = References.Where(r => !traces.Any(r.IsReferencing));
        IEnumerable<Property> properties = Properties.Concat(Join(References, traces).Where(p => !p.IsPrimaryKey));
        return this with { Properties = properties.ToImmutableList(), References = references.ToImmutableList() };
    }

    private static IEnumerable<Property> Join(IEnumerable<Reference> references, ImmutableList<Trace> traces)
    {
        foreach (Reference reference in references)
        {
            if (traces.SingleOrDefault(t => reference.IsReferencing(t)) is not { } trace)
            {
                continue;
            }

            foreach (Property property in trace.Properties)
            {
                yield return property.AsLinked(reference);
            }

            foreach (Property property in Join(trace.References, traces))
            {
                yield return property.AsLinked(reference);
            }
        }
    }
}