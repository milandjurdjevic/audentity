using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Audentity;

public record PropertyTrace(string Name, string? CurrentValue, string? OriginalValue, bool IsPrimaryKey)
{
    internal static PropertyTrace FromEntry(PropertyEntry entry)
    {
        string? currentValue = entry.CurrentValue?.ToString();
        string? originalValue = entry.OriginalValue?.ToString();
        string name = entry.Metadata.Name;
        return new PropertyTrace(name, currentValue, originalValue, entry.Metadata.IsPrimaryKey());
    }
}