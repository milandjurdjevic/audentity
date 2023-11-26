using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Audentity;

public record Property
{
    public string? CurrentValue { get; init; }
    public string? OriginalValue { get; init; }
    public bool IsPrimaryKey { get; init; }
    public bool IsForeignKey { get; init; }
    public string Name { get; init; } = String.Empty;

    public static Property Create(PropertyEntry entry)
    {
        return new Property
        {
            Name = entry.Metadata.Name,
            CurrentValue = entry.CurrentValue?.ToString(),
            OriginalValue = entry.OriginalValue?.ToString(),
            IsPrimaryKey = entry.Metadata.IsPrimaryKey(),
            IsForeignKey = entry.Metadata.IsForeignKey()
        };
    }
}