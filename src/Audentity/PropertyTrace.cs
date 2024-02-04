using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Audentity;

public record PropertyTrace
{
    private PropertyTrace() { }
    public string? CurrentValue { get; private init; }
    public string? OriginalValue { get; private init; }
    public string Name { get; private init; } = String.Empty;

    internal static PropertyTrace FromEntry(PropertyEntry entry)
    {
        return new PropertyTrace
        {
            Name = entry.Metadata.Name,
            CurrentValue = entry.CurrentValue?.ToString(),
            OriginalValue = entry.OriginalValue?.ToString()
        };
    }
}