using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Audentity;

public record Property(string Name, string? CurrentValue, string? OriginalValue)
{
    internal static Property FromEntry(PropertyEntry entry)
    {
        string? currentValue = entry.CurrentValue?.ToString();
        string? originalValue = entry.OriginalValue?.ToString();
        string name = entry.Metadata.Name;
        return new Property(name, currentValue, originalValue);
    }
}