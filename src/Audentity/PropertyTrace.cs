using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Audentity;

public record PropertyTrace
{
    private PropertyTrace() { }

    public string? CurrentValue { get; private init; }
    public string? OriginalValue { get; private init; }
    public bool IsPrimaryKey { get; private init; }
    public string Name { get; private init; } = String.Empty;

    internal static PropertyTrace Create(PropertyEntry entry)
    {
        return new PropertyTrace
        {
            Name = entry.Metadata.Name,
            CurrentValue = entry.CurrentValue?.ToString(),
            OriginalValue = entry.OriginalValue?.ToString(),
            IsPrimaryKey = entry.Metadata.IsPrimaryKey()
        };
    }
}