using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Audentity;

public record ReferenceProperty : Property
{
    public ReferenceProperty(PropertyEntry entry) : base(entry)
    {
        Owner = entry.Metadata.DeclaringEntityType.Name;
    }

    public string Owner { get; }
}