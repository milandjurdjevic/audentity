using System.Collections.Immutable;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Audentity;

public record Entity(
    Type Type,
    EntityState State,
    IReadOnlyCollection<Property> Properties,
    IReadOnlyCollection<Reference> References)
{
    public static Entity FromEntry(EntityEntry entry)
    {
        ImmutableList<Property> properties =
            entry
                .Properties
                .Select(Property.FromEntry)
                .ToImmutableList();

        ImmutableList<Reference> references =
            entry
                .Navigations
                .SelectMany(Reference.FromEntry)
                .ToImmutableList();

        return new Entity(entry.Entity.GetType(), entry.State, properties, references);
    }
}