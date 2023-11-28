using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Audentity;

public record Property
{
    private Property() { }

    public string? CurrentValue { get; private init; }
    public string? OriginalValue { get; private init; }
    public bool IsPrimaryKey { get; private init; }
    public string Name { get; private init; } = String.Empty;

    internal static Property Create(PropertyEntry entry)
    {
        return new Property
        {
            Name = entry.Metadata.Name,
            CurrentValue = entry.CurrentValue?.ToString(),
            OriginalValue = entry.OriginalValue?.ToString(),
            IsPrimaryKey = entry.Metadata.IsPrimaryKey()
        };
    }

    internal Property AsJoined(Reference reference)
    {
        return this with { Name = $"{reference.Name}/{Name}" };
    }
}