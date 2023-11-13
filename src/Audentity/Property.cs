using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Audentity;

public record Property
{
    public Property(PropertyEntry entry)
    {
        IsPrimaryKey = entry.Metadata.IsPrimaryKey();
        OriginalValue = entry.EntityEntry.State == EntityState.Added ? null : entry.OriginalValue?.ToString();
        CurrentValue = entry.CurrentValue?.ToString();
        Name = entry.Metadata.Name;
    }

    public bool IsPrimaryKey { get; }
    public string? OriginalValue { get; init; }
    public string? CurrentValue { get; }
    public string Name { get; }
}