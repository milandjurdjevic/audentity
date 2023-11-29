using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Audentity;

public record LinkTrace
{
    private LinkTrace() { }
    public string Name { get; private init; } = String.Empty;
    public string Value { get; private init; } = String.Empty;

    internal static LinkTrace Create(IProperty property, object entity)
    {
        return new LinkTrace
        {
            Name = property.Name, Value = property.PropertyInfo?.GetValue(entity)?.ToString() ?? String.Empty
        };
    }

    internal static LinkTrace Create(PropertyEntry entry)
    {
        return new LinkTrace { Name = entry.Metadata.Name, Value = entry.CurrentValue?.ToString() ?? String.Empty };
    }
}