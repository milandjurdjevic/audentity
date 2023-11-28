using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Audentity;

public record Link
{
    private Link() { }
    public string Name { get; private init; } = String.Empty;
    public string Value { get; private init; } = String.Empty;

    public static Link CreateLink(IProperty property, object entity)
    {
        return new Link
        {
            Name = property.Name, Value = property.PropertyInfo?.GetValue(entity)?.ToString() ?? String.Empty
        };
    }

    public static Link CreateLink(PropertyEntry entry)
    {
        return new Link { Name = entry.Metadata.Name, Value = entry.CurrentValue?.ToString() ?? String.Empty };
    }
}