using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Audentity;

public record Link(string Name, string Value)
{
    internal static Link FromProperty(IProperty property, object entity)
    {
        string value = property.PropertyInfo?.GetValue(entity)?.ToString() ?? String.Empty;
        return new Link(property.Name, value);
    }

    internal static Link FromEntry(PropertyEntry entry)
    {
        string value = entry.CurrentValue?.ToString() ?? String.Empty;
        return new Link(entry.Metadata.Name, value);
    }
}