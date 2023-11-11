using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Audentity;

public class Property
{
    private Property(PropertyEntry entry)
    {
        IsPrimaryKey = entry.Metadata.IsPrimaryKey();
        OriginalValue = entry.OriginalValue?.ToString();
        CurrentValue = entry.CurrentValue?.ToString();
        IsModified = entry.IsModified;
        Name = entry.Metadata.Name;
    }

    public bool IsPrimaryKey { get; }
    public string? OriginalValue { get; }
    public string? CurrentValue { get; }
    public bool IsModified { get; }
    public string Name { get; }

    private static Property Collect(PropertyEntry entry)
    {
        return new Property(entry);
    }

    public static IReadOnlyCollection<Property> Collect(EntityEntry entry)
    {
        return entry.Properties
            .Select(Collect)
            .ToArray()
            .AsReadOnly();
    }
}